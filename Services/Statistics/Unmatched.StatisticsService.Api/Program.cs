using Unmatched.StatisticsService.Api.Registration;
using Unmatched.StatisticsService.Domain.Registration;
using Unmatched.StatisticsService.EntityFramework.Registration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.RegisterApiMapping();
builder.Services.RegisterDomainMapping();
builder.Services.RegisterEntityFrameworkMapping();
builder.Services.RegisterServices(builder.Configuration);
builder.Services.RegisterDbContext(builder.Configuration);
builder.Services.RegisterRepositories();
builder.Services.RegisterBackgroundServices();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Services.Migrate();
await app.Services.InitializeAsync();

app.Run();
