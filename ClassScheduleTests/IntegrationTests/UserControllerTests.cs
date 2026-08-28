using ClassSchedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace ClassScheduleTests.IntegrationTests
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly string _ApiBaseUrl = "/api/User/";
        private readonly Guid _InstitutionId = Guid.Parse("02268c71-1e0d-4a3c-8732-15f30d84a6c6");

        public UserControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task Login_Succeeds()
        {
            await InitializeAsync();

            CreateUserDTO newUserDTO = new("TEST ACCOUNT", "DO NOT USE", DateOnly.FromDateTime(DateTime.Now), "TESTUSER_123", "Passw0rdTEST", "test@mail.com", UserRoles.Student);

            await _client.PostAsJsonAsync(_ApiBaseUrl + "create", newUserDTO);

            LoginDTO loginDto = new(newUserDTO.Username, newUserDTO.Password, _InstitutionId);
            var login = await _client.PostAsJsonAsync("/api/User/login", loginDto);

            var json = await login.Content.ReadAsStringAsync();
            UserDTO? resultDTO = JsonConvert.DeserializeObject<UserDTO>(json);

            // Cleanup
            await WithContext(async db =>
            {
                await db.Users.Where(x => x.Id == resultDTO!.Id).ExecuteDeleteAsync();
            });

            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        }

        [Fact]
        public async Task Login_Throws_Unauthorized_Error()
        {
            LoginDTO loginDto = new("NOTHING_TEST", "NOTHING_TEST", _InstitutionId);
            var login = await _client.PostAsJsonAsync("/api/User/login", loginDto);

            Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
        }

        [Fact]
        public async Task Add_User_Succeeds()
        {
            await InitializeAsync();
            CreateUserDTO dto = new("TEST ACCOUNT", "DO NOT USE", DateOnly.FromDateTime(DateTime.Now), "TEST_USER_123", "Passw0rdTEST", "test@test.test", UserRoles.Student);

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // Cleanup
            await WithContext(async db =>
            {
                await db.Users.Where(x => x.Username == dto.Username.ToLower()).ExecuteDeleteAsync();
            });
        }

        [Fact]
        public async Task Add_User_Throws_Conflict_Error()
        {
            await InitializeAsync();

            User user = new("TEST ACCOUNT", "DO NOT USE", DateOnly.FromDateTime(DateTime.Now), "CONFLICT_123".ToLower(), "Passw0rdTEST", "test@test.test", UserRoles.Student, _InstitutionId);

            await WithContext(async db =>
            {
                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
            });

            CreateUserDTO dto = new("TEST ACCOUNT", "DO NOT USE", DateOnly.FromDateTime(DateTime.Now), user.Username, "Passw0rdTEST", "test@mail.com", UserRoles.Student);

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            await WithContext(async db =>
            {
                await db.Users.Where(x => x.Id == user.Id).ExecuteDeleteAsync();
            });

            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }

        [Fact]
        public async Task Add_User_Throws_Badrequest_Error()
        {
            await InitializeAsync();

            CreateUserDTO dto = new("TEST ACCOUNT", "DO NOT USE", DateOnly.FromDateTime(DateTime.Now), "BADREQUEST_123", "Passw0rdTEST", "nomail", UserRoles.Student);

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Add_User_Throws_Unauthorized_Error()
        {
            CreateUserDTO dto = new("TEST ACCOUNT", "DO NOT USE", DateOnly.FromDateTime(DateTime.Now), "Unauthorized_123", "Passw0rdTEST", "test@mail.com", UserRoles.Student);

            var result = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", dto);

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task Update_User_Succeeds()
        {
            await InitializeAsync();

            CreateUserDTO newUserDTO = new("TEST ACCOUNT", "DO NOT USE", DateOnly.FromDateTime(DateTime.Now), "TESTUSER_123", "Passw0rdTEST", "test@mail.com", UserRoles.Student);

            var postResult = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", newUserDTO);

            LoginDTO loginDto = new(newUserDTO.Username, newUserDTO.Password, _InstitutionId);
            var login = await _client.PostAsJsonAsync("/api/User/login", loginDto);

            UpdateUserInformationDTO dto = new("Updated Test", "Account", DateOnly.FromDateTime(DateTime.Now), "valid@mail.com");

            var result = await _client.PatchAsJsonAsync(_ApiBaseUrl + "update", dto);

            var json = await result.Content.ReadAsStringAsync();
            UserDTO? resultDTO = JsonConvert.DeserializeObject<UserDTO>(json);

            await WithContext(async db =>
            {
                await db.Users.Where(x => x.Id == resultDTO!.Id).ExecuteDeleteAsync();
            });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(resultDTO);
            Assert.Equal("Updated Test", resultDTO.FirstName);
            Assert.Equal("valid@mail.com", resultDTO.Email);
        }

        [Fact]
        public async Task Update_User_Throws_BadRequest_Error()
        {
            await InitializeAsync();

            CreateUserDTO newUserDTO = new("TEST ACCOUNT", "DO NOT USE", DateOnly.FromDateTime(DateTime.Now), "TESTUSER_123", "Passw0rdTEST", "test@mail.com", UserRoles.Student);

            var postResult = await _client.PostAsJsonAsync(_ApiBaseUrl + "create", newUserDTO);

            LoginDTO loginDto = new(newUserDTO.Username, newUserDTO.Password, _InstitutionId);
            var login = await _client.PostAsJsonAsync("/api/User/login", loginDto);

            UpdateUserInformationDTO dto = new("FAILED Updated Test", "Account", DateOnly.FromDateTime(DateTime.Now), "invalid-mail");

            var result = await _client.PatchAsJsonAsync(_ApiBaseUrl + "update", dto);

            var json = await result.Content.ReadAsStringAsync();
            UserDTO? resultDTO = JsonConvert.DeserializeObject<UserDTO>(json);

            await WithContext(async db =>
            {
                await db.Users.Where(x => x.Id == resultDTO!.Id).ExecuteDeleteAsync();
            });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Change_User_Credentials_Succeeds()
        {
            await InitializeAsync();

            CreateUserDTO newUserDTO = new("TEST ACCOUNT", "DO NOT USE", DateOnly.FromDateTime(DateTime.Now), "TESTUSER_123", "Passw0rdTEST", "test@mail.com", UserRoles.Student);

            await _client.PostAsJsonAsync(_ApiBaseUrl + "create", newUserDTO);

            LoginDTO loginDto = new(newUserDTO.Username, newUserDTO.Password, _InstitutionId);
            var login = await _client.PostAsJsonAsync("/api/User/login", loginDto);

            ChangeUserCredentialsDTO dto = new("new_username123", newUserDTO.Password, "NEW-PASSWORD");

            var result = await _client.PatchAsJsonAsync(_ApiBaseUrl + "Change-User-Credentials", dto);

            LoginDTO updatedLoginDTO = new(dto.Username, dto.NewPassword, _InstitutionId);
            var updatedLogin = await _client.PostAsJsonAsync("/api/User/login", updatedLoginDTO);

            var updatedLoginJson = await updatedLogin.Content.ReadAsStringAsync();
            UserDTO? updatedUserDTO = JsonConvert.DeserializeObject<UserDTO>(updatedLoginJson);

            await WithContext(async db =>
            {
                await db.Users.Where(x => x.Id == updatedUserDTO!.Id).ExecuteDeleteAsync();
            });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(HttpStatusCode.OK, updatedLogin.StatusCode);
        }

        [Fact]
        public async Task Change_User_Credentials_Throws_Conflict_Error()
        {
            await InitializeAsync();

            User user = new("TEST ACCOUNT", "DO NOT USE", DateOnly.FromDateTime(DateTime.Now), "CONFLICT_123".ToLower(), "Passw0rdTEST", "test@test.test", UserRoles.Student, _InstitutionId);

            await WithContext(async db =>
            {
                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
            });

            CreateUserDTO newUserDTO = new("TEST ACCOUNT", "DO NOT USE", DateOnly.FromDateTime(DateTime.Now), "TESTUSER_123", "Passw0rdTEST", "test@mail.com", UserRoles.Student);

            await _client.PostAsJsonAsync(_ApiBaseUrl + "create", newUserDTO);

            LoginDTO loginDto = new(newUserDTO.Username, newUserDTO.Password, _InstitutionId);
            var login = await _client.PostAsJsonAsync("/api/User/login", loginDto);

            ChangeUserCredentialsDTO dto = new("CONFLICT_123", newUserDTO.Password, "NEW-PASSWORD");

            var result = await _client.PatchAsJsonAsync(_ApiBaseUrl + "Change-User-Credentials", dto);

            var loginJson = await login.Content.ReadAsStringAsync();
            UserDTO? userDTO = JsonConvert.DeserializeObject<UserDTO>(loginJson);

            await WithContext(async db =>
            {
                await db.Users.Where(x => x.Id == userDTO!.Id).ExecuteDeleteAsync();
                await db.Users.Where(x => x.Id == user.Id).ExecuteDeleteAsync();
            });

            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }
    }
}
