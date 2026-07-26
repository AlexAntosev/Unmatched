namespace Unmatched.UI.BlazorServer.Services;

using System.Net;
using System.Text.RegularExpressions;

using Amazon.S3;
using Amazon.S3.Model;

using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

public class ImageUploadService(IAmazonS3 s3Client, IOptions<MinioOptions> options) : IImageUploadService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".webp", ".gif"
    };

    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

    public async Task<string> SaveAsync(string category, string entityName, string? currentImageFileName, Stream content, string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"Unsupported image type '{extension}'.");
        }

        var slug = Slugify(entityName);
        var fileName = await ResolveFileNameAsync(category, slug, extension, currentImageFileName);

        if (!ContentTypeProvider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        await s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = options.Value.Bucket,
            Key = BuildKey(category, fileName),
            InputStream = content,
            ContentType = contentType
        });

        return fileName;
    }

    public async Task<bool> ExistsAsync(string category, string fileName)
    {
        try
        {
            await s3Client.GetObjectMetadataAsync(options.Value.Bucket, BuildKey(category, fileName));
            return true;
        }
        catch (AmazonS3Exception e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private async Task<string> ResolveFileNameAsync(string category, string slug, string extension, string? currentImageFileName)
    {
        var candidate = $"{slug}{extension}";
        if (candidate == currentImageFileName || !await ExistsAsync(category, candidate))
        {
            return candidate;
        }

        var suffix = 2;
        string next;
        do
        {
            next = $"{slug}-{suffix}{extension}";
            suffix++;
        }
        while (next != currentImageFileName && await ExistsAsync(category, next));

        return next;
    }

    private static string BuildKey(string category, string fileName) => $"{category}/{fileName}";

    private static string Slugify(string name)
    {
        var lower = name.ToLowerInvariant().Replace("'", string.Empty).Replace(".", string.Empty);
        var slug = Regex.Replace(lower, "[^a-z0-9]+", "-").Trim('-');
        return string.IsNullOrEmpty(slug) ? "image" : slug;
    }
}
