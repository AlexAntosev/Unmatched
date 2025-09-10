namespace Unmatched.MatchService.Domain.Mapping;

using AutoMapper;

using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Dto.Catalog;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Extensions;
using Unmatched.MatchService.Domain.Mapping.ValueResolvers;

public class DomainMapper : Profile
{
    public DomainMapper()
    {
        CreateMap<MatchEntity, Match>().ReverseMap();
        CreateMap<MatchEntity, MatchLog>()
            .ForMember(x => x.MatchId, opt => opt.MapFrom(s => s.Id))
            .ForMember(dest => dest.MapName, opt => opt.MapFrom<MapNameResolver>())
            .ReverseMap();
        CreateMap<FighterEntity, Fighter>().ForMember(dest => dest.Hero, opt => opt.MapFrom<FighterHeroResolver>()).ReverseMap();
        CreateMap<TitleEntity, Title>().ReverseMap();
        CreateMap<CatalogSidekickDto, FighterSidekick>().ReverseMap();
        CreateMap<CatalogHeroDto, FighterHero>().ForMember(dest => dest.Sidekicks, opt => opt.MapFrom<FighterSidekickResolver>()).ReverseMap();
        CreateMap<TournamentEntity, Tournament>().ReverseMap();
    }

    //private string TryGetMapName(Map? map)
    //{
    //    return map?.Name ?? "<forgotten>";
    //}

    private string TryGetTournamentName(TournamentEntity? tournament, Stage? stage)
    {
        var tournamentName = tournament?.Name ?? "<unranked>";
        if (stage is not null)
        {
            var stageName = stage.Value.GetStageName();
            tournamentName += $" ({stageName})";
        }

        return tournamentName;
    }
}