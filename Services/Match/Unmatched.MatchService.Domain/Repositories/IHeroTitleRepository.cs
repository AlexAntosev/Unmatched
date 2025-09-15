namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface IHeroTitleRepository 
{
    Task AddOrUpdateAsync(HeroTitleEntity model);
}
