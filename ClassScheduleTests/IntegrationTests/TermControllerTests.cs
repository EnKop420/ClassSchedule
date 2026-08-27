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

        public TermControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task Add_Term_Succeeds()
        {
            await InitializeAsync();
            CreateTermDTO dto = new("Test Term", new DateOnly(1111, 01, 01), new DateOnly(1111, 02, 01));

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            Assert.True(result.IsSuccessStatusCode);

            var json = await result.Content.ReadAsStringAsync();
            TermDTO? resultDTO = JsonConvert.DeserializeObject<TermDTO>(json);

            Assert.NotNull(resultDTO);
            Assert.Equal(dto.Name, resultDTO.Name);
            Assert.Equal(dto.StartDate, resultDTO.StartDate);
            Assert.Equal(dto.EndDate, resultDTO.EndDate);

            // Cleanup
            await WithContext(async db =>
            {
                await db.Terms.Where(x => x.Id == resultDTO.Id).ExecuteDeleteAsync();
            });
        }

        [Fact]
        public async Task Add_Term_BadRequest()
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
            Term toDeleteValue = new("Test Term TO DELETE", new DateOnly(1111, 01, 01), new DateOnly(1111, 02, 01), Guid.Parse("02268c71-1e0d-4a3c-8732-15f30d84a6c6"));
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

        [Fact]
        public async Task GetAll_Terms_Succeeds()
        {
            await InitializeAsync();

            List<TermDTO>? terms = await _client.GetFromJsonAsync<List<TermDTO>>(_ApiBaseUrl + "get-all");

            Assert.NotNull(terms);
            Assert.NotEmpty(terms);
            Assert.Equal(4, terms.Count);
        }
    }
}