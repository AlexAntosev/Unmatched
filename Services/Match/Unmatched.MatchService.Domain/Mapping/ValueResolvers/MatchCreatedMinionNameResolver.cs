namespace Unmatched.MatchService.Domain.Mapping.ValueResolvers;

using AutoMapper;
using Unmatched.MatchService.Contracts.Kafka;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Entities;

public class MatchCreatedMinionNameResolver(ICatalogMinionCache minionCache) : IValueResolver<MatchMinionEntity, MatchCreated.MatchMinion, string?>
{
    public string? Resolve(MatchMinionEntity source, MatchCreated.MatchMinion destination, string? destMember, ResolutionContext context)
    {
        return minionCache.GetAsync(source.MinionId).GetAwaiter().GetResult()?.Name;
    }
}
