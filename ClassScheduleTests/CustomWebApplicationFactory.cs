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
                // 1. Find and remove the existing DbContext registration from Program.cs
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<SchoolDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // 2. Add DbContext using your Test Database Connection String
                services.AddDbContext<SchoolDbContext>(options =>
                {
                    options.UseNpgsql(TestConnectionString);
                });

                // 3. (Optional) Ensure the database is created and migrated
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();

                // Ensures clean state / applies migrations
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated(); // Or db.Database.Migrate();
            });
        }
    }
}
