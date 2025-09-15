namespace Unmatched.MatchService.Domain.Mapping.ValueResolvers;

using AutoMapper;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Models;

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