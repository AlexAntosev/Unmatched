namespace Unmatched.MatchService.Domain.Mapping.ValueResolvers;

using AutoMapper;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Models;

public class MatchVillainNameResolver(ICatalogVillainCache villainCache) : IValueResolver<MatchVillainEntity, MatchVillain, string?>
{
    public string? Resolve(MatchVillainEntity source, MatchVillain destination, string? destMember, ResolutionContext context)
    {
        return villainCache.GetAsync(source.VillainId).GetAwaiter().GetResult()?.Name;
    }
}
