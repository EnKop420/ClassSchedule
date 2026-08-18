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
        public async Task Create_Institution()
        {
            // Arrange
            CreateInstitutionDTO dto = new("TEC");

            // Act
            await _service.CreateInstitution(dto);

            // Assert
            Assert.True(_context.Institutions.Any(i => i.Name == dto.Name));
        }

        [Fact]
        public async Task Update_Institution_Return_InstitutionDTO()
        {
            Institution institution = new("TEC");
            await _context.Institutions.AddAsync(institution);
            await _context.SaveChangesAsync();

            if (_context.Institutions.Any(i => i.Id == institution.Id))
            {
                InstitutionDTO dto = new(institution.Id, "Updated TEC");
                InstitutionDTO updatedDTO = await _service.UpdateInstitution(dto);
                Assert.True(_context.Institutions.Any(i => i.Name == dto.Name && i.Id == institution.Id));
            }

        }

        [Fact]
        public async Task Delete_Institution()
        {
            Institution institution = new("TEC");
            await _context.Institutions.AddAsync(institution);
            await _context.SaveChangesAsync();

            if (_context.Institutions.Any(i => i.Id == institution.Id))
            {
                await _service.DeleteInstitution(institution.Id);
            }

            Assert.True(_context.Institutions.Any(i => i.Id == institution.Id) == false);
        }

        public async Task Get_Institution_Return_InstitutionDTO()
        {
            Institution institution = new("TEC");
            await _context.Institutions.AddAsync(institution);
            await _context.SaveChangesAsync();

            InstitutionDTO? institutionDTO = await _service.GetInstitutionById(institution.Id);

            Assert.NotNull(institutionDTO);
            Assert.Equal(institution.Id, institutionDTO.Id);
        }

        [Fact]
        public async 
    }
}
