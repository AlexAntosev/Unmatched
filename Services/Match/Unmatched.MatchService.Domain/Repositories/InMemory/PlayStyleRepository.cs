namespace Unmatched.MatchService.Domain.Repositories.InMemory;

using System;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

public class PlayStyleRepository : BaseInMemoryRepository<PlayStyle>, IPlayStyleRepository
{
    protected override Guid GetId(PlayStyle model)
    {
        return model.HeroId;
    }
}