namespace Unmatched.Services;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;
using Unmatched.HttpClients.Contracts;
using Unmatched.Services.Contracts;

public class TitleService(IMatchClient client) : ITitleService
{
    public Task AddAsync(TitleDto title)
        => client.AddTitleAsync(title);

    public Task<IEnumerable<TitleDto>> GetAsync()
        => client.GetTitlesAsync();

    public Task DeleteAsync(Guid id)
        => client.DeleteTitleAsync(id);

    public Task MergeAsync(Guid titleId, IEnumerable<Guid> heroesIds)
        => client.MergeTitleAsync(titleId, heroesIds);

    public Task<IEnumerable<HeroTitleAssignDto>> GetHeroesForTitleAssign(Guid titleId)
        => client.GetHeroesForTitleAssignAsync(titleId);
}
