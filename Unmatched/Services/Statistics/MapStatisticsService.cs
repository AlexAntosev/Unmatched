namespace Unmatched.Services.Statistics;

using System;
using AutoMapper;
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
        var maps = await _unitOfWork.Maps.GetAsync();
        var mapMatches = await _unitOfWork.Matches.GetFinishedAsync();

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
        
        var mapMatches = await _unitOfWork.Matches.GetFinishedByMapIdAsync(mapId);
        
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
        var mapMatches = await _unitOfWork.Matches.GetFinishedByMapIdAsync(mapId);

        var matchLogs = new List<MatchLogDto>();
        
        foreach (var match in mapMatches)
        {
            var matchLog = _mapper.Map<MatchLogDto>(match);

            var fighters = await _unitOfWork.Fighters.GetByMatchIdAsync(matchLog.MatchId);
            
            matchLog.Fighters = _mapper.Map<List<FighterDto>>(fighters);

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }
}
