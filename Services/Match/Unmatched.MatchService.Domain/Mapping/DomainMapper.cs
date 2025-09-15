namespace Unmatched.MatchService.Domain.Mapping;

using AutoMapper;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Communication.Player.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Extensions;
using Unmatched.MatchService.Domain.Mapping.ValueResolvers;
using Unmatched.MatchService.Domain.Models;

public class DomainMapper : Profile
{
    public DomainMapper()
    {
        CreateMap<MatchEntity, Match>()
            .ForMember(dest => dest.Map, opt => opt.MapFrom<MapResolver>())
            .ReverseMap()
            .ForMember(x => x.MapId, opt => opt.MapFrom(s => s.Map!.Id));

        CreateMap<MatchEntity, MatchLog>()
            .ForMember(x => x.MatchId, opt => opt.MapFrom(s => s.Id))
            .ForMember(dest => dest.MapName, opt => opt.MapFrom<MapNameResolver>())
            .ReverseMap();
        CreateMap<FighterEntity, Fighter>()
            .ForMember(dest => dest.Hero, opt => opt.MapFrom<FighterHeroResolver>())
            .ForMember(dest => dest.Player, opt => opt.MapFrom<FighterPlayerResolver>())
            .ReverseMap()
            .ForMember(x => x.PlayerId, opt => opt.MapFrom(s => s.Player!.Id));
        CreateMap<TitleEntity, Title>().ReverseMap();
        CreateMap<CatalogSidekickDto, FighterSidekick>().ReverseMap();
        CreateMap<CatalogHeroDto, FighterHero>().ReverseMap();
        CreateMap<PlayerDto, FighterPlayer>().ReverseMap();
        CreateMap<TournamentEntity, Tournament>().ReverseMap();
        CreateMap<RatingEntity, Rating>();
    }

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