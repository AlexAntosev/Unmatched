using Unmatched.CatalogService.Api.Registration;
using Unmatched.CatalogService.Domain.Registration;
using Unmatched.CatalogService.EntityFramework.Registration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterDbContext(builder.Configuration);
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();
builder.Services.RegisterApiMapping();
builder.Services.RegisterDomainMapping();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger(); // generates /swagger/v1/swagger.json
    app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            c.RoutePrefix = string.Empty; 
        });
}

app.UseHttpsRedirection();

app.UseErrorHandling();
app.UseRequestLogging();
app.UseAuthorization();

app.MapControllers();

app.Services.Migrate();

app.Run();
