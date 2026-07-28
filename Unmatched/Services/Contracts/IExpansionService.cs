namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;

public interface IExpansionService
{
    Task<IEnumerable<ExpansionDto>> GetAsync();

    Task<string> UpdateImageAsync(Guid expansionId, string imageFileName);
}
