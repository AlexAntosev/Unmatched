namespace Unmatched.MatchService.Domain.Repositories;

using System;

public interface IUnitOfWork : IDisposable
{

    IFighterRepository Fighters { get; }


    IMatchRepository Matches { get; }


    IRatingRepository Ratings { get; }

    ITournamentRepository Tournaments { get; }

    ITitleRepository Titles { get; }

    IHeroTitleRepository HeroTitles { get; }

    Task SaveChangesAsync();
}