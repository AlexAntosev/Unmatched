namespace Unmatched.Services;

using Unmatched.Dtos;

public interface ITitleService
{
    Task AddAsync(TitleDto title);

    Task<IEnumerable<TitleDto>> GetAsync();

    Task DeleteAsync(Guid id);

    Task AssignAsync(Guid titleId, Guid heroId);
}
