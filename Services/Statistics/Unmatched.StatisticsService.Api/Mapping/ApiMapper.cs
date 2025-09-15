namespace Unmatched.StatisticsService.Api.Mapping
{
    using AutoMapper;

    using Unmatched.StatisticsService.Api.Dto;
    using Unmatched.StatisticsService.Domain.Models;

    public class ApiMapper : Profile
    {
        public ApiMapper()
        {
            CreateMap<HeroStats, HeroStatsDto>();
            CreateMap<PlayerStats, PlayerStatsDto>();
        }
    }
}
