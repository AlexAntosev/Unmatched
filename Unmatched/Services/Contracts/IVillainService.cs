namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;

public interface IVillainService
{
    Task<IEnumerable<VillainDto>> GetAsync();
}
