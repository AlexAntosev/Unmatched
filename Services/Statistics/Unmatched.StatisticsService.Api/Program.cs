using Unmatched.StatisticsService.Api.Registration;
using Unmatched.StatisticsService.Domain.Registration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.RegisterApiMapping();
builder.Services.RegisterDomainMapping();
builder.Services.RegisterServices(builder.Configuration);

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

app.Run();
