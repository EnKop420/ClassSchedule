
using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository;
using SchoolScheduleLibrary.Repository.Generic;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Service;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Encryption;
using SchoolScheduleLibrary.Utilities.Encryption.Interface;
using StackExchange.Redis;

namespace ClassSchedule
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Allowing calls from the Blazor app to the API (CORS)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazorApp", policy =>
                {
                    policy.WithOrigins("https://localhost:7010") // Blazor app URL
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            Console.WriteLine(builder.Environment.EnvironmentName);

            string redisConnection = builder.Configuration.GetConnectionString("RedisConnection");
            //string postgresConnection = builder.Configuration.GetConnectionString("DevelopmentConnection");
            string postgresConnection = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<SchoolDbContext>(options =>
                options.UseNpgsql(postgresConnection));

            // DI Scoped
            builder.Services.AddScoped<IEncryptionHandler, EncryptionHandler>();

            // DI Services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IInstitutionService, InstitutionService>();
            builder.Services.AddScoped<ISubjectService, SubjectService>();
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddScoped<IPeriodService, PeriodService>();
            builder.Services.AddScoped<ITermService, TermService>();
            builder.Services.AddScoped<IHoldService, HoldService>();
            builder.Services.AddScoped<ILessonTemplateService, LessonTemplateService>();

            // Database Repositories Scoped
            builder.Services.AddScoped<IRedisRepository, RedisRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Generic Scoped
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(redisConnection);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapGet("/", () => Results.Redirect("/swagger"));
                app.MapOpenApi();
            }

            // Enable CORS for the Blazor app
            app.UseCors("AllowBlazorApp");

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
