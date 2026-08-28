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
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace ClassScheduleTests.IntegrationTests
{
    public class NonTeachingDayControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly string _ApiBaseUrl = "/api/NonTeachingDay/";
        private readonly Guid _InstitutionId = Guid.Parse("02268c71-1e0d-4a3c-8732-15f30d84a6c6");

        public NonTeachingDayControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            LoginDTO dto = new("test", "Passw0rd", _InstitutionId);
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
        public async Task Add_NonTeachingDay_Succeeds()
        {
            await InitializeAsync();
            CreateNonTeachingDayDTO dto = new(new DateOnly(1111, 01, 01), new DateOnly(1111, 02, 01), "Test NonTeachingDay");

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            Assert.True(result.IsSuccessStatusCode);

            var json = await result.Content.ReadAsStringAsync();
            NonTeachingDayDTO? resultDTO = JsonConvert.DeserializeObject<NonTeachingDayDTO>(json);

            Assert.NotNull(resultDTO);
            Assert.Equal(dto.Reason, resultDTO.Reason);
            Assert.Equal(dto.StartDate, resultDTO.StartDate);
            Assert.Equal(dto.EndDate, resultDTO.EndDate);

            // Cleanup
            await WithContext(async db =>
            {
                await db.NonTeachingDays.Where(x => x.Id == resultDTO.Id).ExecuteDeleteAsync();
            });
        }

        [Fact]
        public async Task Add_NonTeachingDay_Throws_BadRequest_Error()
        {
            await InitializeAsync();

            // Start time is after End Time.
            CreateNonTeachingDayDTO dto = new(new DateOnly(1111, 02, 01), new DateOnly(1111, 01, 01), "Fail NonTeachingDay");

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            Assert.False(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Delete_NonTeachingDay_Succeeds()
        {
            await InitializeAsync();
            NonTeachingDay toDeleteValue = new(new DateOnly(1111, 01, 01), new DateOnly(1111, 02, 01), "Test NonTeachingDay TO DELETE", _InstitutionId);
            await WithContext(async db =>
            {
                await db.NonTeachingDays.AddAsync(toDeleteValue);
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
