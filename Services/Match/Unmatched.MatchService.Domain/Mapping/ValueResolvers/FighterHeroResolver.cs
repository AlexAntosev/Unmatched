namespace Unmatched.MatchService.Domain.Mapping.ValueResolvers;

using AutoMapper;

using Unmatched.MatchService.Domain.Catalog;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Models;

public class FighterHeroResolver(ICatalogHeroCache heroCache, IMapper mapper) : IValueResolver<FighterEntity, Fighter, FighterHero?>
{

    public FighterHero Resolve(FighterEntity source, Fighter destination, FighterHero? destMember, ResolutionContext context)
    {
        return mapper.Map<FighterHero>(heroCache.GetAsync(source.HeroId).GetAwaiter().GetResult());
    }
}