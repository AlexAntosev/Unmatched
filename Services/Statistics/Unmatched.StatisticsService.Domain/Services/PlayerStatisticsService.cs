// namespace Unmatched.StatisticsService.Domain.Services;
//
// using System;
//
// using AutoMapper;
//
// using Unmatched.StatisticsService.Domain.Models;
// using Unmatched.StatisticsService.Domain.Services.Contracts;
//
// public class PlayerStatisticsService : IPlayerStatisticsService
// {
//     private readonly IUnitOfWork _unitOfWork;
//     private readonly IMapper _mapper;
//
//     public PlayerStatisticsService(IUnitOfWork unitOfWork, IMapper mapper)
//     {
//         _unitOfWork = unitOfWork;
//         _mapper = mapper;
//     }
//     
//     public async Task<IEnumerable<PlayerStats>> GetPlayersStatisticsAsync()
//     {
//         var playerEntities = await _unitOfWork.Players.GetAsync();
//         var players = _mapper.Map<List<PlayerDto>>(playerEntities);
//         var fighters = await _unitOfWork.Fighters.GetFromFinishedMatchesAsync();
//
//         var statistics = new List<PlayerStats>();
//
//         foreach (var player in players)
//         {
//             var playerFighters = fighters.Where(x => x.PlayerId.Equals(player.Id)).OrderByDescending(x => x.Match.Date).ToArray();
//
//             var playerStatistics = new PlayerStats
//                 {
//                     Name = player,
//                     PlayerId = player.Id,
//                     TotalMatches = playerFighters.Length,
//                     TotalWins = playerFighters.Count(x => x.IsWinner),
//                     TotalLooses = playerFighters.Count(x => x.IsWinner == false),
//                     LastMatchPoints = playerFighters.FirstOrDefault()?.MatchPoints ?? 0
//                 };
//
//             statistics.Add(playerStatistics);
//         }
//
//         return statistics;
//     }
//     
//     public async Task<PlayerStats> GetPlayerStatisticsAsync(Guid playerId)
//     {
//         var playerEntity = await _unitOfWork.Players.GetByIdAsync(playerId);
//         var player = _mapper.Map<PlayerDto>(playerEntity);
//         var playerFighters = await _unitOfWork.Fighters.GetFromFinishedMatchesByPlayerIdAsync(playerId);
//         
//         var statistics = new PlayerStats
//             {
//                 Player = player,
//                 PlayerId = player.Id,
//                 TotalMatches = playerFighters.Count,
//                 TotalWins = playerFighters.Count(x => x.IsWinner),
//                 TotalLooses = playerFighters.Count(x => x.IsWinner == false),
//                 LastMatchPoints = playerFighters.FirstOrDefault()?.MatchPoints ?? 0
//             };
//         
//         return statistics;
//     }
// }
