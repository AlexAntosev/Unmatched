namespace Unmatched.CatalogService.Api.Mapping;

using System;

using AutoMapper;

using Unmatched.CatalogService.Api.Dto;
using Unmatched.CatalogService.EntityFramework.Entities;

public class CatalogMapper : Profile
{
    public CatalogMapper()
    {
        CreateMap<Hero, HeroDto>().ReverseMap();
    }
}

   
