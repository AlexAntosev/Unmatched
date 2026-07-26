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
        CreateMap<Expansion, ExpansionDto>()
            .ForMember(d => d.HeroNames, o => o.MapFrom(s => s.Heroes.Select(h => h.Name)));
        CreateMap<Villain, VillainDto>()
            .ForMember(d => d.ExpansionName, o => o.MapFrom(s => s.Expansion != null ? s.Expansion.Name : null));
        CreateMap<Minion, MinionDto>()
            .ForMember(d => d.ExpansionName, o => o.MapFrom(s => s.Expansion != null ? s.Expansion.Name : null));
    }
}

   
