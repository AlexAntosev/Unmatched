namespace Unmatched.MatchService.Domain.Mapping.ValueResolvers;

using AutoMapper;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Models;

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
