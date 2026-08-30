using ClassSchedule;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class PeriodControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly string _ApiBaseUrl = "/api/Period/";
        private readonly Guid _InstitutionId = Guid.Parse("02268c71-1e0d-4a3c-8732-15f30d84a6c6");

        public PeriodControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task Add_Period_Succeeds()
        {
            await InitializeAsync();
            CreatePeriodDTO dto = new("Test Modul123123", TimeOnly.Parse("08:00:00"), TimeOnly.Parse("09:00:00"));

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            await WithContext(async db =>
            {
                Period? period = await db.Periods.FirstOrDefaultAsync(x => x.Name == dto.Name);

                // Cleanup
                await db.Periods.Where(x => x.Id == period!.Id).ExecuteDeleteAsync();

                Assert.True(result.IsSuccessStatusCode);
                Assert.NotNull(period);
                Assert.Equal(dto.Name, period.Name);
                Assert.Equal(dto.StartTime, period.StartTime);
                Assert.Equal(dto.EndTime, period.EndTime);
            });

        }

        [Fact]
        public async Task Add_Period_BadRequest()
        {
            await InitializeAsync();

            // Start time is after End Time.
            CreatePeriodDTO dto = new("Fail Modul", TimeOnly.Parse("09:00:00"), TimeOnly.Parse("08:00:00"));

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            Assert.False(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Delete_Period_Succeeds()
        {
            await InitializeAsync();
            Period toDeleteValue = new("Test Modul TO DELETE", TimeOnly.Parse("12:00:00"), TimeOnly.Parse("13:00:00"), _InstitutionId);
            await WithContext(async db =>
            {
                await db.Periods.AddAsync(toDeleteValue);
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
