using ClassSchedule.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Generic;
using SchoolScheduleLibrary.Service;
using SchoolScheduleLibrary.Utilities.Encryption;
using SchoolScheduleLibrary.Utilities.Response;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace ClassScheduleTests.UnitTests.Controllers
{
    public class InstitutionControllerTests
    {
        private DbContextOptions<SchoolDbContext> _options;
        private SchoolDbContext _context;
        private readonly InstitutionController _controller;
        private readonly InstitutionService _service;
        private readonly GenericRepository<Institution> _repository;
        public InstitutionControllerTests()
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
            _controller = new InstitutionController(_service);
        }

        [Fact]
        public async Task Create_Institution_Return_Ok()
        {
            // Arrange
            CreateInstitutionDTO dto = new("TEC");

            // Act
            var result = await _controller.CreateInstitution(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Get_Institution_Return_DTO()
        {
            // Arrange
            Institution institution = new("TEC");
            Guid id = institution.Id;
            await _context.Institutions.AddAsync(institution);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetInstitution(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            InstitutionDTO dto = Assert.IsType<InstitutionDTO>(okResult.Value);
            Assert.NotNull(dto);
            Assert.Equal(id, dto.Id);
            Assert.Equal(institution.Name, dto.Name);
        }

        [Fact]
        public async Task Update_Institution_Return_Updated_DTO()
        {
            // Arrange
            string oldName = "TEC";
            Institution institution = new(oldName);
            Guid id = institution.Id;
            await _context.Institutions.AddAsync(institution);
            await _context.SaveChangesAsync();

            InstitutionDTO dto = new(id, "Technical Education Copenhagen");

            // Act
            var result = await _controller.UpdateInstitution(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            InstitutionDTO updatedDTO = Assert.IsType<InstitutionDTO>(okResult.Value);
            Assert.NotNull(updatedDTO);
            Assert.Equal(id, updatedDTO.Id);
            Assert.NotEqual(oldName, updatedDTO.Name);
            Assert.Equal(dto.Name, updatedDTO.Name);
        }

        [Fact]
        public async Task Delete_Institution_Return_Ok()
        {
            // Arrange
            Institution institution = new("TEC");
            Guid id = institution.Id;
            await _context.Institutions.AddAsync(institution);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteInstitution(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
            Assert.True(_context.Institutions.Any(i => i.Id == id) == false);
        }

        [Fact]
        public async Task Get_All_Institution_Return_List()
        {
            List<Institution> institutions = new List<Institution>
            {
                new("TEC"),
                new("Munkegaard"),
                new("Havard")
            };
            await _context.Institutions.AddRangeAsync(institutions);
            await _context.SaveChangesAsync();

            var result = await _controller.GetAllInstitutions();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            List<InstitutionDTO> dtos = Assert.IsType<List<InstitutionDTO>>(okResult.Value);
            Assert.Equal(3, dtos.Count);
            Assert.True(dtos.All(d => institutions.Any(i => i.Id == d.Id)));
        }

        [Fact]
        public async Task Get_Return_NotFound_Error()
        {
            // Act
            var result = await _controller.GetInstitution(Guid.NewGuid());

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Update_Return_NotFound_Error()
        {
            // Act
            var result = await _controller.UpdateInstitution(new InstitutionDTO(Guid.NewGuid(), "TEST!!!!"));

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Delete_Return_NotFound_Error()
        {
            // Act
            var result = await _controller.DeleteInstitution(Guid.NewGuid());

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
