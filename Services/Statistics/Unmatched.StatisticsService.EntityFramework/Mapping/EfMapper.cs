namespace Unmatched.StatisticsService.EntityFramework.Mapping
{
    using AutoMapper;

    using Unmatched.StatisticsService.Domain.Models;
    using Unmatched.StatisticsService.EntityFramework.Entities;

    public class EfMapper : Profile
    {
        public EfMapper()
        {
            CreateMap<HeroStats, HeroStatsEntity>();
            CreateMap<MapStats, MapStatsEntity>();
        }
    }
}
