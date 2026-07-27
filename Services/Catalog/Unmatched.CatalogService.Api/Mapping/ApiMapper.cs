namespace Unmatched.CatalogService.Api.Mapping;

using System;

using AutoMapper;

using Unmatched.CatalogService.Api.Dto;
using Unmatched.CatalogService.Domain.Entities;

public class ApiMapper : Profile
{
    public ApiMapper()
    {
        CreateMap<Hero, HeroDto>()
            .ForMember(d => d.ExpansionName, o => o.MapFrom(s => s.Expansion != null ? s.Expansion.Name : null))
            .ReverseMap();
        CreateMap<PlayStyle, PlayStyleDto>().ReverseMap();
        CreateMap<Map, MapDto>()
            .ForMember(d => d.ExpansionName, o => o.MapFrom(s => s.Expansion != null ? s.Expansion.Name : null))
            .ReverseMap();
        CreateMap<Sidekick, SidekickDto>().ReverseMap();
        CreateMap<Expansion, ExpansionDto>();
        CreateMap<Hero, ExpansionContentDto>();
        CreateMap<Villain, ExpansionContentDto>();
        CreateMap<Minion, ExpansionContentDto>();
        CreateMap<Villain, VillainDto>()
            .ForMember(d => d.ExpansionName, o => o.MapFrom(s => s.Expansion != null ? s.Expansion.Name : null));
        CreateMap<Minion, MinionDto>()
            .ForMember(d => d.ExpansionName, o => o.MapFrom(s => s.Expansion != null ? s.Expansion.Name : null));
    }
}

   
