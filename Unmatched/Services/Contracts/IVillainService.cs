namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;

public interface IVillainService
{
    Task<IEnumerable<VillainDto>> GetAsync();

    Task<string> UpdateImageAsync(Guid villainId, string imageFileName);
}
