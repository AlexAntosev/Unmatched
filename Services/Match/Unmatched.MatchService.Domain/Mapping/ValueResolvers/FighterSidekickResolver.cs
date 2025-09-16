namespace Unmatched.MatchService.Domain.Mapping.ValueResolvers;

using AutoMapper;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Models;

public class FighterSidekickResolver(ICatalogSidekickCache sidekickCache, IMapper mapper) : IValueResolver<CatalogHeroDto, FighterHero, IEnumerable<FighterSidekick>>
{

    public IEnumerable<FighterSidekick> Resolve(CatalogHeroDto source, FighterHero destination, IEnumerable<FighterSidekick> destMember, ResolutionContext context)
    {
        return mapper.Map<IEnumerable<FighterSidekick>>(sidekickCache.GetByHeroAsync(source.Id).GetAwaiter().GetResult());
    }
}