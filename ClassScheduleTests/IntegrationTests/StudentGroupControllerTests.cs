using ClassSchedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace ClassScheduleTests.IntegrationTests
{
    public class StudentGroupControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly string _ApiBaseUrl = "/api/StudentGroup/";

        public StudentGroupControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            LoginDTO dto = new("test", "Passw0rd", Guid.Parse("02268c71-1e0d-4a3c-8732-15f30d84a6c6"));
            var login = await _client.PostAsJsonAsync("/api/User/login", dto);
            login.EnsureSuccessStatusCode();
        }

        public async Task WithContext(Func<SchoolDbContext, Task> action)
        {
            using var scope = _factory.Services.CreateScope();
            SchoolDbContext context = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
            await action(context);
        }

        [Fact]
        public async Task Add_StudentGroup_Succeeds()
        {
            await InitializeAsync();
            List<Guid> studentIds = new List<Guid>
            {
                Guid.Parse("6cb8a39d-d5c7-4d8b-8985-244378877b9f"),
                Guid.Parse("b77e34c8-b315-414d-95fe-700994163531"),
                Guid.Parse("6e915cef-75be-4d72-8056-fceafd340ff4"),
                Guid.Parse("b43f205e-d3b8-41d2-bfd8-793e72cbf61e")
            };
            CreateStudentGroupDTO dto = new("Klasse TEST", studentIds);

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            var json = await result.Content.ReadAsStringAsync();
            StudentGroupDTO? resultDTO = JsonConvert.DeserializeObject<StudentGroupDTO>(json);

            Assert.True(result.IsSuccessStatusCode);
            Assert.NotNull(resultDTO);
            Assert.Equal(dto.Name, resultDTO.Name);
            // Cleanup
            await WithContext(async db =>
            {
                await db.StudentGroups.Where(x => x.Id == resultDTO.Id).ExecuteDeleteAsync();
            });
        }

        [Fact]
        public async Task Delete_StudentGroup_Succeeds()
        {
            await InitializeAsync();
            StudentGroup toDeleteValue1 = new("Test Klasse TO DELETE", Guid.Parse("02268c71-1e0d-4a3c-8732-15f30d84a6c6"));
            StudentGroupMember toDeleteValue2 = new(toDeleteValue1.Id, Guid.Parse("5f69666a-28f3-48ad-ae3c-a22c832558b2"));

            await WithContext(async db =>
            {
                await db.StudentGroups.AddAsync(toDeleteValue1);
                await db.StudentGroupMembers.AddAsync(toDeleteValue2);
                await db.SaveChangesAsync();
            });

            var result = await _client.DeleteAsync(_ApiBaseUrl + $"delete?id={toDeleteValue1.Id}");

            var errorExpected = await _client.GetAsync(_ApiBaseUrl + $"get?id={toDeleteValue1.Id}");

            Assert.True(result.IsSuccessStatusCode);

            await WithContext(async db =>
            {
                Assert.Null(
                    await db.StudentGroupMembers.FirstOrDefaultAsync(sgm => 
                    sgm.StudentGroupId == toDeleteValue1.Id 
                    && sgm.StudentId == toDeleteValue2.StudentId)
                );
            });

            Assert.False(errorExpected.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, errorExpected.StatusCode);
        }

        [Fact]
        public async Task GetAll_StudentGroups_Succeeds()
        {
            await InitializeAsync();

            List<StudentGroupDTO>? StudentGroups = await _client.GetFromJsonAsync<List<StudentGroupDTO>>(_ApiBaseUrl + "get-all");

            Assert.NotNull(StudentGroups);
            Assert.NotEmpty(StudentGroups);
            Assert.Equal(2, StudentGroups.Count);
        }
    }
}
