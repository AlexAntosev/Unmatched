namespace Unmatched.MatchService.Domain.Mapping.ValueResolvers;

using AutoMapper;
using Unmatched.MatchService.Domain.Communication.Player;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Models;

public class FighterPlayerResolver(IPlayerCache playerCache, IMapper mapper) : IValueResolver<FighterEntity, Fighter, FighterPlayer?>
{
    public FighterPlayer Resolve(
        FighterEntity source,
        Fighter destination,
        FighterPlayer? destMember,
        ResolutionContext context)
    {
        return mapper.Map<FighterPlayer>(playerCache.GetAsync(source.PlayerId).GetAwaiter().GetResult());
    }
}
