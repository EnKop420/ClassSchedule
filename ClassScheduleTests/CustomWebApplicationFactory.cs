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
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove everything EF/DbContext-related that the app registered
                var toRemove = services.Where(d =>
                    d.ServiceType == typeof(DbContextOptions<SchoolDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(SchoolDbContext) ||
                    (d.ServiceType.Namespace?.StartsWith("Microsoft.EntityFrameworkCore") ?? false)
                ).ToList();

                foreach (var d in toRemove)
                    services.Remove(d);

                services.AddDbContext<SchoolDbContext>(options =>
                    options.UseInMemoryDatabase("DummyDB"));

            });
        }
    }
}
