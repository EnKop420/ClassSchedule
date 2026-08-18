using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Generic;
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
        private readonly GenericRepository<Institution> _repository;

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

            _repository = new GenericRepository<Institution>(_context);
        }

        [Fact]
        public async Task Get_Succeeds()
        {

        }

        [Fact]
        public async Task Get_All_Succeeds()
        {

        }

        [Fact]
        public async Task Add_Succeeds()
        {

        }

        [Fact]
        public async Task AddRange_Succeeds()
        {

        }

        [Fact]
        public async Task Update_Succeeds()
        {

        }

        [Fact]
        public async Task Delete_Succeeds()
        {

        }
    }
}
