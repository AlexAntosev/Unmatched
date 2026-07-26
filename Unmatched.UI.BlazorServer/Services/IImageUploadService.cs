namespace Unmatched.UI.BlazorServer.Services;

public interface IImageUploadService
{
    /// <summary>
    /// Saves the uploaded image bytes into wwwroot/images/{category}, deriving the file name from
    /// entityName. Overwrites the file if it's the same one this entity already owns; otherwise picks
    /// a disambiguated name so other entities' (or unrelated) images on disk are never clobbered.
    /// Returns the file name that was actually written.
    /// </summary>
    Task<string> SaveAsync(string category, string entityName, string? currentImageFileName, Stream content, string originalFileName);

    Task<bool> ExistsAsync(string category, string fileName);
}
