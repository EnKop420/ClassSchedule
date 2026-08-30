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
    public class SubjectControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly string _ApiBaseUrl = "/api/Subject/";
        private readonly Guid _InstitutionId = Guid.Parse("02268c71-1e0d-4a3c-8732-15f30d84a6c6");

        public SubjectControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task InitializeAsync()
        {
            LoginDTO dto = new("test", "Passw0rd", _InstitutionId);
            var login = await _client.PostAsJsonAsync("/api/User/login", dto);
            login.EnsureSuccessStatusCode();
        }

        private async Task WithContext(Func<SchoolDbContext, Task> action)
        {
            using var scope = _factory.Services.CreateScope();
            SchoolDbContext context = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
            await action(context);
        }

        [Fact]
        public async Task Add_Subject_Succeeds()
        {
            await InitializeAsync();
            CreateSubjectDTO dto = new("Test Fag123123");

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            var json = await result.Content.ReadAsStringAsync();

            await WithContext(async db =>
            {
                Subject? subject = await db.Subjects.FirstOrDefaultAsync(x => x.Name == dto.Name);

                // Cleanup
                await db.Subjects.Where(s => s.Id == subject!.Id).ExecuteDeleteAsync();

                Assert.True(result.IsSuccessStatusCode);
                Assert.NotNull(subject);
                Assert.Equal("Test Fag123123", subject.Name);
            });

        }

        [Fact]
        public async Task Delete_Subject_Succeeds()
        {
            await InitializeAsync();
            Subject toDeleteValue = new("Biologi", _InstitutionId);
            await WithContext(async db =>
            {
                await db.Subjects.AddAsync(toDeleteValue);
                await db.SaveChangesAsync();
            });

            var result = await _client.DeleteAsync(_ApiBaseUrl + $"delete?id={toDeleteValue.Id}");

            var errorExpected = await _client.GetAsync(_ApiBaseUrl + $"get?id={toDeleteValue.Id}");

            Assert.True(result.IsSuccessStatusCode);
            Assert.False(errorExpected.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, errorExpected.StatusCode);
        }
    }
}
