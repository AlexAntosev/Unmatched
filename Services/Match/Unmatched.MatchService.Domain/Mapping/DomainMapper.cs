namespace Unmatched.MatchService.Domain.Mapping;

using AutoMapper;

using Unmatched.MatchService.Contracts.Kafka;
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
        // HeroTitles is a child entity collection on the Title entity side but a flat Holders list on
        // the Title model side - same shape mismatch as Tournament/Participants, so the read direction
        // is explicit and the write direction leaves HeroTitles for the evaluator/service to build.
        CreateMap<TitleEntity, Title>()
            .ForMember(d => d.Holders, o => o.MapFrom(s => s.HeroTitles))
            .ReverseMap()
            .ForMember(d => d.HeroTitles, o => o.Ignore());
        CreateMap<HeroTitleEntity, TitleHolder>()
            .ForMember(d => d.HeroId, o => o.MapFrom(s => s.HeroesId));
        CreateMap<CatalogHeroDto, HeroTitleAssign>()
            .ForMember(dest => dest.IsAssigned, opt => opt.Ignore());
        CreateMap<CatalogSidekickDto, FighterSidekick>().ReverseMap();
        CreateMap<CatalogHeroDto, FighterHero>().ReverseMap();
        CreateMap<PlayerDto, FighterPlayer>().ReverseMap();
        // Participants/TournamentTitles are child entity collections on the TournamentEntity side but
        // flat id/kind lists on the Tournament model side - convention mapping can't bridge that shape
        // difference, so the read direction is explicit and the write direction (used only when
        // creating/updating scalars) leaves the child collections for TournamentService to build.
        CreateMap<TournamentEntity, Tournament>()
            .ForMember(d => d.ParticipantHeroIds, o => o.MapFrom(s => s.Participants.Select(p => p.HeroId)))
            .ForMember(d => d.TitleKinds, o => o.MapFrom(s => s.TournamentTitles.Select(t => t.Kind)))
            .ReverseMap()
            .ForMember(d => d.Participants, o => o.Ignore())
            .ForMember(d => d.TournamentTitles, o => o.Ignore());
        CreateMap<RatingEntity, Rating>();
        CreateMap<TournamentAwardEntity, TournamentAward>();
        CreateMap<MatchVillainEntity, MatchVillain>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom<MatchVillainNameResolver>())
            .ReverseMap();
        CreateMap<MatchMinionEntity, MatchMinion>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom<MatchMinionNameResolver>())
            .ReverseMap();


        CreateMap<MatchEntity, MatchCreated>();
        CreateMap<FighterEntity, MatchCreated.Fighter>()
            .ForMember(dest => dest.ResultRating, opt => opt.Ignore());
        CreateMap<MatchVillainEntity, MatchCreated.MatchVillain>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom<MatchCreatedVillainNameResolver>());
        CreateMap<MatchMinionEntity, MatchCreated.MatchMinion>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom<MatchCreatedMinionNameResolver>());
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