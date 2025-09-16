namespace Unmatched.MatchService.Domain.Repositories.InMemory;

using System;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

public class PlayStyleRepository : BaseInMemoryRepository<PlayStyleEntity>, IPlayStyleRepository
{
    protected override Guid GetId(PlayStyleEntity model)
    {
        return model.HeroId;
    }
}