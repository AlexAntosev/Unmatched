namespace Unmatched.StatisticsService.Domain.Mapping
{
    using AutoMapper;
    using Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;
    using Unmatched.StatisticsService.Domain.Models;

    public class DomainMapper : Profile
    {
        public DomainMapper()
        {
            CreateMap<CatalogHeroDto, HeroStats>()
                .ForMember(x => x.HeroId, c => c.MapFrom(s => s.Id));
            CreateMap<CatalogMapDto, MapStats>()
                .ForMember(x => x.MapId, c => c.MapFrom(s => s.Id));
        }
    }
}
