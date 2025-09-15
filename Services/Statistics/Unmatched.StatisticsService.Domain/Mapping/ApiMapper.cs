namespace Unmatched.StatisticsService.Domain.Mapping
{
    using AutoMapper;

    using Unmatched.StatisticsService.Domain.Catalog.Dto;
    using Unmatched.StatisticsService.Domain.Models;

    public class DomainMapper : Profile
    {
        public DomainMapper()
        {
            CreateMap<CatalogHeroDto, HeroStats>()
                .ForMember(x => x.HeroId, c => c.MapFrom(s => s.Id));
        }
    }
}
