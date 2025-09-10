namespace Unmatched.MatchService.Domain.Mapping.ValueResolvers;

using AutoMapper;

using Unmatched.MatchService.Domain.Catalog;
using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Entities;

public class FighterHeroResolver(ICatalogHeroCache heroCache, IMapper mapper) : IValueResolver<FighterEntity, Fighter, FighterHero?>
{

    public FighterHero Resolve(FighterEntity source, Fighter destination, FighterHero? destMember, ResolutionContext context)
    {
        return mapper.Map<FighterHero>(heroCache.GetAsync(source.HeroId).GetAwaiter().GetResult());
    }
}

public class MapNameResolver(ICatalogMapCache mapCache, IMapper mapper) : IValueResolver<MatchEntity, MatchLog, string>
{

    public string Resolve(MatchEntity source, MatchLog destination, string destMember, ResolutionContext context)
    {
        var mapName = "<forgotten>";
        if (source.MapId != null)
        {
            var map = mapCache.GetAsync(source.MapId.Value).GetAwaiter().GetResult();
            mapName = map?.Name ?? mapName;
        }

        return mapName;
    }
}