using System.Net;

using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

using Unmatched.Registration;
using Unmatched.UI.BlazorServer;
using Unmatched.UI.BlazorServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddHubOptions(options =>
{
    // Default SignalR message size (32 KB) is too small for original-quality image uploads.
    options.MaximumReceiveMessageSize = 20 * 1024 * 1024;
});
builder.Services.RegisterServices(builder.Configuration);
builder.Services.RegisterMapping();
builder.Services.AddBlazorBootstrap();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<RatingRecalculationStateNotifier>();
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();
builder.Services.AddScoped<NavState>();
builder.Services.AddScoped<NavCountsService>();
builder.Services.AddScoped<AppSettingsService>();

builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection("Minio"));
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var minioOptions = sp.GetRequiredService<IOptions<MinioOptions>>().Value;
    var config = new AmazonS3Config
    {
        ServiceURL = $"{(minioOptions.UseSsl ? "https" : "http")}://{minioOptions.Endpoint}",
        ForcePathStyle = true
    };
    return new AmazonS3Client(minioOptions.AccessKey, minioOptions.SecretKey, config);
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapGet("/images/{category}/{fileName}", async (string category, string fileName, IAmazonS3 s3Client, IOptions<MinioOptions> minioOptions) =>
{
    try
    {
        var response = await s3Client.GetObjectAsync(minioOptions.Value.Bucket, $"{category}/{fileName}");
        return Results.Stream(response.ResponseStream, response.Headers.ContentType);
    }
    catch (AmazonS3Exception e) when (e.StatusCode == HttpStatusCode.NotFound)
    {
        return Results.NotFound();
    }
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await SeedImagesFromWwwRootAsync(app);

app.Run();

static async Task SeedImagesFromWwwRootAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var s3Client = scope.ServiceProvider.GetRequiredService<IAmazonS3>();
    var minioOptions = scope.ServiceProvider.GetRequiredService<IOptions<MinioOptions>>().Value;
    var uploadService = scope.ServiceProvider.GetRequiredService<IImageUploadService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    if (!await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, minioOptions.Bucket))
    {
        await s3Client.PutBucketAsync(minioOptions.Bucket);
    }

    var seedRoot = Path.Combine(app.Environment.WebRootPath, "images");
    if (!Directory.Exists(seedRoot))
    {
        return;
    }

    var contentTypeProvider = new FileExtensionContentTypeProvider();
    foreach (var category in Directory.EnumerateDirectories(seedRoot))
    {
        var categoryName = Path.GetFileName(category);
        foreach (var filePath in Directory.EnumerateFiles(category))
        {
            var fileName = Path.GetFileName(filePath);
            if (await uploadService.ExistsAsync(categoryName, fileName))
            {
                continue;
            }

            contentTypeProvider.TryGetContentType(filePath, out var contentType);
            try
            {
                await s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = minioOptions.Bucket,
                    Key = $"{categoryName}/{fileName}",
                    FilePath = filePath,
                    ContentType = contentType ?? "application/octet-stream"
                });
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Failed to seed image {Category}/{FileName} into MinIO", categoryName, fileName);
            }
        }
    }
}
