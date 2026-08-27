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
    public class RoomControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly string _ApiBaseUrl = "/api/Room/";

        public RoomControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task Add_Room_Succeeds()
        {
            await InitializeAsync();
            CreateRoomDTO dto = new("Test Room", 100);

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            Assert.True(result.IsSuccessStatusCode);

            var json = await result.Content.ReadAsStringAsync();
            RoomDTO? resultDTO = JsonConvert.DeserializeObject<RoomDTO>(json);

            Assert.NotNull(resultDTO);
            Assert.Equal(dto.Name, resultDTO.Name);
            Assert.Equal(dto.Capacity, resultDTO.Capacity);

            // Cleanup
            await WithContext(async db =>
            {
                await db.Rooms.Where(x => x.Id == resultDTO.Id).ExecuteDeleteAsync();
            });
        }

        [Fact]
        public async Task Delete_Room_Succeeds()
        {
            await InitializeAsync();
            Room toDeleteValue = new("Test Room TO DELETE", 25, Guid.Parse("02268c71-1e0d-4a3c-8732-15f30d84a6c6"));
            await WithContext(async db =>
            {
                await db.Rooms.AddAsync(toDeleteValue);
                await db.SaveChangesAsync();
            });

            var result = await _client.DeleteAsync(_ApiBaseUrl + $"delete?id={toDeleteValue.Id}");

            var errorExpected = await _client.GetAsync(_ApiBaseUrl + $"get?id={toDeleteValue.Id}");

            Assert.True(result.IsSuccessStatusCode);
            Assert.False(errorExpected.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, errorExpected.StatusCode);
        }

        public async Task GetAll_Rooms_Succeeds()
        {
            await InitializeAsync();

            List<RoomDTO>? rooms = await _client.GetFromJsonAsync<List<RoomDTO>>(_ApiBaseUrl + "get-all");

            Assert.NotNull(rooms);
            Assert.NotEmpty(rooms);
            Assert.Equal(4, rooms.Count);
        }
    }
}
