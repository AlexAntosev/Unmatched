namespace Unmatched.CatalogService.Domain.Mapping;

using AutoMapper;

using Unmatched.CatalogService.Contracts.Kafka.Events;
using Unmatched.CatalogService.Domain.Entities;

public class DomainMapper : Profile
{
    public DomainMapper()
    {
        CreateMap<PlayStyle, PlayStyleUpdated>();
    }
}
