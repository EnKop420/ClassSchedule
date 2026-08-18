using ClassSchedule.Controllers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Generic;
using SchoolScheduleLibrary.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassScheduleTests.UnitTests.Services
{
    public class InstitutionServiceTests
    {
        private DbContextOptions<SchoolDbContext> _options;
        private SchoolDbContext _context;
        private readonly InstitutionService _service;
        private readonly GenericRepository<Institution> _repository;
        public InstitutionServiceTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();


            _options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseSqlite(connection)
                .Options;

            _context = new SchoolDbContext(_options);

            _context.Database.EnsureDeletedAsync().Wait();
            _context.Database.EnsureCreatedAsync().Wait();

            _repository = new GenericRepository<Institution>(_context);
            _service = new InstitutionService(_repository);
        }

        [Fact]
        public async Task Create_Institution_Return_InstitutionDTO()
        {
            // Arrange
            CreateInstitutionDTO dto = new("TEC");

        }
    }
}
