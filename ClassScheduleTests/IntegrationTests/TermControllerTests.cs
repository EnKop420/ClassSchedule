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
    public class TermControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly string _ApiBaseUrl = "/api/Term/";
        private readonly Guid _InstitutionId = Guid.Parse("02268c71-1e0d-4a3c-8732-15f30d84a6c6");
        public TermControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task Add_Term_Succeeds()
        {
            await InitializeAsync();
            CreateTermDTO dto = new("Test Term", new DateOnly(1111, 01, 01), new DateOnly(1111, 02, 01));

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            Assert.True(result.IsSuccessStatusCode);

            var json = await result.Content.ReadAsStringAsync();
            TermDTO? resultDTO = JsonConvert.DeserializeObject<TermDTO>(json);

            // Cleanup
            await WithContext(async db =>
            {
                await db.Terms.Where(x => x.Id == resultDTO!.Id).ExecuteDeleteAsync();
            });

            Assert.NotNull(resultDTO);
            Assert.Equal(dto.Name, resultDTO.Name);
            Assert.Equal(dto.StartDate, resultDTO.StartDate);
            Assert.Equal(dto.EndDate, resultDTO.EndDate);
        }

        [Fact]
        public async Task Add_Term_Throws_BadRequest_Error()
        {
            await InitializeAsync();

            // Start time is after End Time.
            CreateTermDTO dto = new("Fail Term", new DateOnly(1111, 10, 01), new DateOnly(1111, 01, 01));

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            Assert.False(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Delete_Term_Succeeds()
        {
            await InitializeAsync();
            Term toDeleteValue = new("Test Term TO DELETE", new DateOnly(1111, 01, 01), new DateOnly(1111, 02, 01), _InstitutionId);
            await WithContext(async db =>
            {
                await db.Terms.AddAsync(toDeleteValue);
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