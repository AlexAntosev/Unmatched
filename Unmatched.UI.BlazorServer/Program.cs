using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Unmatched.EntityFramework.Registration;
using Unmatched.Registration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.RegisterServices();
builder.Services.RegisterMapping();
builder.Services.RegisterDataInitializers();
builder.Services.RegisterDbContext(builder.Configuration);
builder.Services.RegisterRepositories();

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

app.Run();