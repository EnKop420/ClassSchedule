using ClassSchedule;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolScheduleLibrary.Context;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace ClassScheduleTests
{
    public class CustomWebApplicationFactory<TProgram> 
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        private const string TestConnectionString = "Host=94.130.71.38;Port=3003;Database=test_school_schedule;Username=postgres;Password=secretpassword420;SSL Mode=Disable";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var toRemove = services.Where(d =>
                    d.ServiceType == typeof(DbContextOptions<SchoolDbContext>) ||
                    d.ServiceType == typeof(SchoolDbContext) ||
                    d.ServiceType == typeof(DbConnection)).ToList();

                foreach (var d in toRemove)
                    services.Remove(d);

                services.AddDbContext<SchoolDbContext>(options =>
                {
                    options.UseNpgsql(TestConnectionString);
                });
            });
        }

        public async Task InitializeAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
            await db.Database.EnsureCreatedAsync(); // or MigrateAsync()
        }

        public new Task DisposeAsync() => Task.CompletedTask;
    }
}
