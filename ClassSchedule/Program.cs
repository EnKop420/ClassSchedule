
using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Service;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Encryption;
using SchoolScheduleLibrary.Utilities.Encryption.Interface;
using StackExchange.Redis;

namespace ClassSchedule
{
    public partial class Program
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

            string redisConnection = builder.Configuration.GetConnectionString("RedisConnection") ?? throw new Exception("No redis connection string!");
            //string postgresConnection = builder.Configuration.GetConnectionString("DevelopmentConnection") ?? throw new Exception("No Postgres connection string!");
            string postgresConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("No Postgres connection string!");

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
            builder.Services.AddScoped<ILessonGenerationService, LessonGeneratorService>();
            builder.Services.AddScoped<INonTeachingDayService, NonTeachingDayService>();
            builder.Services.AddScoped<IHoldMemberService, HoldMemberService>();
            builder.Services.AddScoped<IScheduleService, ScheduleService>();
            builder.Services.AddScoped<IStudentGroupService, StudentGroupService>();
            builder.Services.AddScoped<IStudentGroupMemberService, StudentGroupMemberService>();
            builder.Services.AddScoped<ILessonService, LessonService>();

            // Database Repositories Scoped
            builder.Services.AddScoped<IRedisRepository, RedisRepository>();
            builder.Services.AddScoped<ILessonRepository, LessonRepository>();

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
