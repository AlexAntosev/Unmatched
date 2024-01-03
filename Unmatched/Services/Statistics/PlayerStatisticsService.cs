namespace Unmatched.Services.Statistics;

using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unmatched.Dtos;
using Unmatched.Repositories;

public class PlayerStatisticsService : IPlayerStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PlayerStatisticsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<PlayerStatisticsDto>> GetPlayersStatisticsAsync()
    {
        var players = await _unitOfWork.Players.Query().ToListAsync();
        var fighters = await _unitOfWork.Fighters.Query().Include(x => x.Match).Where(f => !f.Match.IsPlanned).ToListAsync();

        var statistics = new List<PlayerStatisticsDto>();

        foreach (var player in players)
        {
            var playerFighters = fighters.Where(x => x.PlayerId.Equals(player.Id)).OrderByDescending(x => x.Match.Date).ToArray();

            var playerStatistics = new PlayerStatisticsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.Name,
                    TotalMatches = playerFighters.Length,
                    TotalWins = playerFighters.Count(x => x.IsWinner),
                    TotalLooses = playerFighters.Count(x => x.IsWinner == false),
                    LastMatchPoints = playerFighters.FirstOrDefault()?.MatchPoints ?? 0
                };

            statistics.Add(playerStatistics);
        }

        return statistics;
    }
    
    public async Task<PlayerStatisticsDto> GetPlayerStatisticsAsync(Guid playerId)
    {
        var player = await _unitOfWork.Players.GetByIdAsync(playerId);
        
        var playerFighters = await _unitOfWork.Fighters
            .Query()
            .Include(x => x.Match)
            .Where(x => x.PlayerId.Equals(player.Id) && !x.Match.IsPlanned)
            .OrderByDescending(x => x.Match.Date)
            .ToListAsync();
        
        var statistics = new PlayerStatisticsDto
            {
                PlayerId = player.Id,
                PlayerName = player.Name,
                TotalMatches = playerFighters.Count,
                TotalWins = playerFighters.Count(x => x.IsWinner),
                TotalLooses = playerFighters.Count(x => x.IsWinner == false),
                LastMatchPoints = playerFighters.FirstOrDefault()?.MatchPoints ?? 0
            };
        
        return statistics;
    }
    
    public async Task<IEnumerable<MatchLogDto>> GetPlayerMatchesAsync(Guid playerId)
    {
        var playerMatches = await _unitOfWork.Matches
            .Query()
            .Where(m => m.Fighters.Any(f => f.PlayerId == playerId) && !m.IsPlanned)
            .Include(x => x.Map)
            .Include(x => x.Tournament)
            .ToListAsync();

        var matchLogs = new List<MatchLogDto>();
        
        foreach (var match in playerMatches)
        {
            var matchLog = _mapper.Map<MatchLogDto>(match);

            var fighters = await _unitOfWork.Fighters
                .Query()
                .Where(x => matchLog.MatchId == x.MatchId)
                .Include(x => x.Player)
                .Include(x => x.Hero)
                .Select(fighter => _mapper.Map<FighterDto>(fighter))
                .ToListAsync();
            
            matchLog.Fighters = fighters;

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }
}
