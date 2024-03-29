using Syncfusion.Blazor;

using Unmatched.EntityFramework.Registration;
using Unmatched.Initializer.Registration;
using Unmatched.Registration;
using Unmatched.UI.BlazorServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.RegisterServices();
builder.Services.RegisterMapping();
builder.Services.RegisterInitializers();
builder.Services.RegisterDbContext(builder.Configuration);
builder.Services.RegisterRepositories();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddBlazorBootstrap();
builder.Services.AddScoped<NotificationService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Services.Migrate();
app.Run();
