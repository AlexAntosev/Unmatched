namespace Unmatched.MatchService.Domain.Repositories;

using System;

public interface IUnitOfWork : IDisposable
{

    IFighterRepository Fighters { get; }


    IMatchRepository Matches { get; }


    IRatingRepository Ratings { get; }

    IRatingRecalculationStateRepository RatingRecalculationState { get; }

    ITournamentRepository Tournaments { get; }

    ITournamentParticipantRepository TournamentParticipants { get; }

    ITournamentAwardRepository TournamentAwards { get; }

    ITitleRepository Titles { get; }

    IHeroTitleRepository HeroTitles { get; }

    Task SaveChangesAsync();
}