using Hangfire;
using LMS;
using LMS.DTOs.Common;
using LMS.DTOs.Employee;
using LMS.Interface;
using LMS.Repository;
using LMS.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console() // Console (for dev & container logs)
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // daily rolling file
    .WriteTo.Seq("http://localhost:5341") // optional: log server
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog();  // 👈 Replace default logging with Serilog
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add DbContext with SQL Server
builder.Services.AddDbContext<LMSEFCoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<JWTResponse>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JWTResponse>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });
builder.Services.AddHangfire(config => config.UseSqlServerStorage(
    builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:59766")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularClient");
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");
//BackgroundJob.Enqueue<LeaveService>(s => s.AllocateMonthlyLeavesAsync());
//RecurringJob.AddOrUpdate<LeaveService>(
//    "monthly-leave-allocation",   // recurringJobId
//    service => service.AllocateMonthlyLeavesAsync(),
//    "0 0 1 * *"                   // CRON: 1st of every month at midnight
//);
app.MapControllers();
// ?? Schedule Recurring Jobs


// 4?? Register recurring jobs only ONCE here
RecurringJob.AddOrUpdate<ILeaveService>(
    "annual-leave-allocation",
    s => s.AllocateAnnualLeaveAsync(),
    "5 0 1 1 *" // every Jan 1st at 00:05
);

RecurringJob.AddOrUpdate<ILeaveService>(
    "monthly-leave-allocation",
    s => s.AllocateMonthlyLeavesAsync(),
    "5 0 1 * *" // 1st of every month at 00:05
);
//BackgroundJob.Enqueue<ILeaveService>(s => s.AllocateMonthlyLeavesAsync());

app.Run();
