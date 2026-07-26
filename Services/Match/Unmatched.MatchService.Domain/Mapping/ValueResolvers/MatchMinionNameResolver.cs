namespace Unmatched.MatchService.Domain.Mapping.ValueResolvers;

using AutoMapper;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Models;

public class MatchMinionNameResolver(ICatalogMinionCache minionCache) : IValueResolver<MatchMinionEntity, MatchMinion, string?>
{
    public string? Resolve(MatchMinionEntity source, MatchMinion destination, string? destMember, ResolutionContext context)
    {
        return minionCache.GetAsync(source.MinionId).GetAwaiter().GetResult()?.Name;
    }
}
