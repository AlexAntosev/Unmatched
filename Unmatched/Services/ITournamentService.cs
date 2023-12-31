﻿namespace Unmatched.Services;

using System;

using Unmatched.Dtos;
using Unmatched.Entities;
using Unmatched.Enums;

public interface ITournamentService
{
    Task<TournamentDto> AddAsync(TournamentDto dto);
    
    Task UpdateAsync(Guid id, IEnumerable<MatchDto> matches, Stage stage);

    Task<IEnumerable<TournamentDto>> GetAsync();
    
    Task<TournamentDto> GetAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task CreateInitialPlannedMatchesAsync(TournamentDto tournament);

    Task CreateNextStagePlannedMatchesAsync(TournamentDto tournament);

    Task CreateThirdPlaceDeciderMatchAsync(TournamentDto tournament);

    Task CreateGrandFinalMatchesAsync(TournamentDto tournament);
}
