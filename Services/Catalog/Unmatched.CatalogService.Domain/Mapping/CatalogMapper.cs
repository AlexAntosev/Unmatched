namespace Unmatched.CatalogService.Domain.Mapping;

using System;

using AutoMapper;

using Unmatched.CatalogService.Contracts.Kafka.Events;
using Unmatched.CatalogService.Domain.Entities;

public class CatalogMapper : Profile
{
    public CatalogMapper()
    {
        CreateMap<PlayStyle, PlayStyleUpdated>();
    }
}

   
