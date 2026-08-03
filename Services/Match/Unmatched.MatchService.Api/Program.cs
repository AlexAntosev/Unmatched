using Unmatched.MatchService.Api.Registration;
using Unmatched.MatchService.Domain.Registration;
using Unmatched.MatchService.Domain.Services;
using Unmatched.MatchService.EntityFramework.Registration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.RegisterApiMapping();
builder.Services.RegisterDomainMapping();
builder.Services.RegisterDbContext(builder.Configuration);
builder.Services.RegisterRepositories();
builder.Services.RegisterServices(builder.Configuration);

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

// Self-heals a completed tournament's awards whenever its participants no longer match them - e.g. right
// after FixTournamentParticipantsMismatchDetection corrects a tournament's roster, or after restoring a
// database backup that reverted an earlier recomputation.
using (var scope = app.Services.CreateScope())
{
    var tournamentService = scope.ServiceProvider.GetRequiredService<ITournamentService>();
    await tournamentService.ReconcileCompletionAwardsAsync();
}

app.Run();
