namespace Unmatched.Services.Statistics;

using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unmatched.Dtos;
using Unmatched.Repositories;

public class MapStatisticsService : IMapStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MapStatisticsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<MapStatisticsDto>> GetMapsStatisticsAsync()
    {
        var maps = await _unitOfWork.Maps.Query().ToListAsync();
        var mapMatches = await _unitOfWork.Matches
            .Query()
            .ToListAsync();

        var statistics = new List<MapStatisticsDto>();

        foreach (var map in maps)
        {
            var mapFighters = mapMatches.Where(x => x.MapId.Equals(map.Id)).OrderByDescending(x => x.Date).ToArray();

            var mapStatistics = new MapStatisticsDto
                {
                    MapId = map.Id,
                    MapName = map.Name,
                    TotalMatches = mapFighters.Length,
                };

            statistics.Add(mapStatistics);
        }

        return statistics;
    }

    

    public async Task<MapStatisticsDto> GetMapStatisticsAsync(Guid mapId)
    {
        var map = await _unitOfWork.Maps.GetByIdAsync(mapId);
        
        var mapMatches = await _unitOfWork.Matches
            .Query()
            .Where(x => x.MapId.Equals(map.Id))
            .OrderByDescending(x => x.Date)
            .ToListAsync();
        
        var statistics = new MapStatisticsDto
            {
                MapId = map.Id,
                MapName = map.Name,
                TotalMatches = mapMatches.Count,
            };
        
        return statistics;
    }
    
    public async Task<IEnumerable<MatchLogDto>> GetMapMatchesAsync(Guid mapId)
    {
        var mapMatches = await _unitOfWork.Matches
            .Query()
            .Where(m => m.MapId == mapId)
            .Include(x => x.Map)
            .Include(x => x.Tournament)
            .ToListAsync();

        var matchLogs = new List<MatchLogDto>();
        
        foreach (var match in mapMatches)
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
