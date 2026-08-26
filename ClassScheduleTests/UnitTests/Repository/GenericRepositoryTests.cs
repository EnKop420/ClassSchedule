using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository;
using SchoolScheduleLibrary.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassScheduleTests.UnitTests.Repository
{
    public class GenericRepositoryTests
    {
        private DbContextOptions<SchoolDbContext> _options;
        private SchoolDbContext _context;
        private readonly GenericRepository<Institution> _institutionRepository;
        private readonly GenericRepository<User> _userRepository;

        public GenericRepositoryTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();


            _options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseSqlite(connection)
                .Options;

            _context = new SchoolDbContext(_options);

            _context.Database.EnsureDeletedAsync().Wait();
            _context.Database.EnsureCreatedAsync().Wait();

            _institutionRepository = new GenericRepository<Institution>(_context);
            _userRepository = new GenericRepository<User>(_context);
        }

        [Fact]
        public async Task Get_Succeeds()
        {
            // Arrange
            await Add_Succeeds();

            // Act
            Institution? institution = await _institutionRepository.Get(i => i.Name == "TEC");

            Assert.NotNull(institution);
            Assert.True(institution.Name == "TEC");
        }

        [Fact]
        public async Task GetAll_Succeeds()
        {
            // Arrange
            await AddRange_Succeeds();

            // Act
            List<Institution> institutions = await _institutionRepository.GetAll();

            // Assert
            Assert.NotEmpty(institutions);
            Assert.Equal(_context.Institutions.Count(), institutions.Count);
        }

        [Fact]
        public async Task GetAll_With_Predicate_Succeeds()
        {
            // Arrange
            await AddRange_Succeeds();

            // Act
            List<Institution> institutions = await _institutionRepository.GetAll(i => i.Name.Length == 3);

            // Assert
            Assert.NotEmpty(institutions);
            Assert.Equal(3, institutions.Count);
            Assert.NotEqual(_context.Institutions.Count(), institutions.Count);
        }

        [Fact]
        public async Task Add_Succeeds()
        {
            // Arrange
            Institution institution = new("TEC");

            // Act
            bool success = await _institutionRepository.Add(institution);

            // Assert
            Assert.True(success);
            Assert.True(_context.Institutions.Any(i => i.Id == institution.Id));
        }

        [Fact]
        public async Task AddRange_Succeeds()
        {
            // Arrange
            List<Institution> institutions = new List<Institution>
            {
                new("TEC"),
                new("HC Ørsted"),
                new("DTU"),
                new("HTX Lyngby"),
                new("STU"),
                new("Munkegaardsskolen"),
                new("Next KBH")
            };

            // Act
            bool succees = await _institutionRepository.AddRange(institutions);

            // Assert
            Assert.True(succees);
            Assert.Equal(7, _context.Institutions.Count());
        }

        [Fact]
        public async Task Update_Succeeds()
        {
            // Arrange
            await Add_Succeeds();
            Institution? institution = await _institutionRepository.Get(i => i.Name == "TEC");
            Assert.NotNull(institution);

            institution.Name = "Harvard";
            // Act
            Institution updatedInstitution = await _institutionRepository.Update(institution);

            // Assert
            Assert.Equal(institution.Id, updatedInstitution.Id);
            Assert.Equal("Harvard", updatedInstitution.Name);
            Assert.True(_context.Institutions.Any(i => i.Name == "Harvard" && i.Id == institution.Id));
        }

        [Fact]
        public async Task Delete_Succeeds()
        {
            // Arrange
            await AddRange_Succeeds();
            int originalCount = _context.Institutions.Count();
            Institution? institution = _context.Institutions.FirstOrDefault(i => i.Name == "STU");
            Assert.NotNull(institution);

            // Act
            bool success = await _institutionRepository.Delete(i => i.Id == institution.Id);

            // Assert
            Assert.True(success);
            Assert.False(_context.Institutions.Any(i => i.Id == institution.Id));
            Assert.False(_context.Institutions.Any(i => i.Name == "STU"));
        }

        [Fact]
        public async Task RemoveRange_Succeeds()
        {
            // Arrange
            await AddRange_Succeeds();
            List<Institution> institutions = await _institutionRepository.GetAll();

            institutions.RemoveAll(i => i.Name.Count() != 3);

            // Act
            bool success = await _institutionRepository.RemoveRange(institutions);

            // Assert
            Assert.Equal(4, _context.Institutions.Count());
            Assert.True(_context.Institutions.Any(i => i.Name == "HC Ørsted"));
            Assert.True(_context.Institutions.Any(i => i.Name == "HTX Lyngby"));
            Assert.True(_context.Institutions.Any(i => i.Name == "Munkegaardsskolen"));
            Assert.True(_context.Institutions.Any(i => i.Name == "Next KBH"));
        }

        [Fact]
        public async Task DoesValueExists_Succeeds()
        {
            await Add_Succeeds();

            Assert.True(await _institutionRepository.DoesValueExist(i => i.Name == "TEC"));
        }

        [Fact]
        public async Task Count_Succeeds()
        {
            await AddRange_Succeeds();

            Assert.Equal(7, await _institutionRepository.Count());
        }

        [Fact]
        public async Task Count_With_Predicate_Succeeds()
        {
            await AddRange_Succeeds();

            Assert.Equal(3, await _institutionRepository.Count(i => i.Name.Length == 3));
        }

        [Fact]
        public async Task Get_With_Include_Succeeds()
        {
            Institution institution = new("TEC");
            await _institutionRepository.Add(institution);
            User newUser = new("Anders", "And", DateOnly.FromDateTime(DateTime.Now), "anders123", "password123", "anders@mail.dk", UserRoles.Admin, institution.Id);
            await _userRepository.Add(newUser);

            User? user = await _userRepository.Get(u => u.Id == newUser.Id, u => u.Institution);

            Assert.NotNull(user);
            Assert.NotNull(user.Institution);
            Assert.Equal("TEC", user.Institution.Name);
        }

        [Fact]
        public async Task GetAll_With_Include_Succeed()
        {
            Institution institution = new("TEC");
            await _institutionRepository.Add(institution);
            List<User> newUsers = new List<User>
            {
                new("Anders", "And", DateOnly.FromDateTime(DateTime.Now), "anders123", "password123", "anders@mail.dk", UserRoles.Admin, institution.Id),
                new("Mickey", "Mouse", DateOnly.FromDateTime(DateTime.Now), "mickey123", "password123", "mickey@mail.dk", UserRoles.Teacher, institution.Id),
                new("Minnie", "Mouse", DateOnly.FromDateTime(DateTime.Now), "minnie123", "password123", "minnie@mail.dk", UserRoles.Teacher, institution.Id),
                new("Fedtmule", "Hund", DateOnly.FromDateTime(DateTime.Now), "fedtmule123", "password123", "fedtmule@mail.dk", UserRoles.Student, institution.Id)
            };

            await _userRepository.AddRange(newUsers);

            List<User> users = await _userRepository.GetAll(u => u.InstitutionId == institution.Id, u => u.Institution);

            Assert.NotEmpty(users);
            Assert.Equal(4, users.Count);
            Assert.Equal("TEC", users[1].Institution.Name);
        }

        [Fact]
        public async Task Add_Fails()
        {
            User user = new("Anders", "And", DateOnly.FromDateTime(DateTime.Now), "anders123", "password123", "anders@mail.dk", UserRoles.Admin, Guid.NewGuid());

            await Assert.ThrowsAsync<SqliteException>(async () =>
            {
                await _userRepository.Add(user);
            });
        }

        [Fact]
        public async Task Update_Fails()
        {
            User user = new("Anders", "And", DateOnly.FromDateTime(DateTime.Now), "anders123", "password123", "anders@mail.dk", UserRoles.Admin, Guid.NewGuid());

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                await _userRepository.Update(user);
            });
        }
    }
}
