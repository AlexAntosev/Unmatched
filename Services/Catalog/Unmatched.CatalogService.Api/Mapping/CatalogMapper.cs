namespace Unmatched.CatalogService.Api.Mapping;

using System;

using AutoMapper;

using Unmatched.CatalogService.Api.Dto;
using Unmatched.CatalogService.Domain.Entities;

public class CatalogMapper : Profile
{
    public CatalogMapper()
    {
        CreateMap<Hero, HeroDto>().ReverseMap();
        CreateMap<PlayStyle, PlayStyleDto>().ReverseMap();
        CreateMap<Map, MapDto>().ReverseMap();
    }
}

   
