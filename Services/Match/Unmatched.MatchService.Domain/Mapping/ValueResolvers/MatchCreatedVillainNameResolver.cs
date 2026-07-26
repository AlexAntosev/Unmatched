namespace Unmatched.MatchService.Domain.Mapping.ValueResolvers;

using AutoMapper;
using Unmatched.MatchService.Contracts.Kafka;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Entities;

public class MatchCreatedVillainNameResolver(ICatalogVillainCache villainCache) : IValueResolver<MatchVillainEntity, MatchCreated.MatchVillain, string?>
{
    public string? Resolve(MatchVillainEntity source, MatchCreated.MatchVillain destination, string? destMember, ResolutionContext context)
    {
        return villainCache.GetAsync(source.VillainId).GetAwaiter().GetResult()?.Name;
    }
}
