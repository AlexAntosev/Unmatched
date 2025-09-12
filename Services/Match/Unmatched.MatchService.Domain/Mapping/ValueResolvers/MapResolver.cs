namespace Unmatched.MatchService.Domain.Mapping.ValueResolvers;

using AutoMapper;

using Unmatched.MatchService.Domain.Catalog;
using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Dto.Catalog;
using Unmatched.MatchService.Domain.Entities;

public class MapResolver(ICatalogMapCache mapCache) : IValueResolver<MatchEntity, Match, CatalogMapDto?>
{

    public CatalogMapDto? Resolve(
        MatchEntity source,
        Match destination,
        CatalogMapDto? destMember,
        ResolutionContext context)
    {

        var map = source.MapId.HasValue ? mapCache.GetAsync(source.MapId.Value).GetAwaiter().GetResult() : null;

        return map;
    }
}
