namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;

public interface IMinionService
{
    Task<IEnumerable<MinionDto>> GetAsync();

    Task<string> UpdateImageAsync(Guid minionId, string imageFileName);
}
