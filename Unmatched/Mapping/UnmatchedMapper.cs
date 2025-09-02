namespace Unmatched.Mapping;

using AutoMapper;

using Unmatched.Data.Entities;
using Unmatched.Data.Enums;
using Unmatched.Dtos;
using Unmatched.Extensions;

public class UnmatchedMapper : Profile
{
    public UnmatchedMapper()
    {
        CreateMap<Player, PlayerDto>().ReverseMap();
        CreateMap<Hero, HeroDto>().ForMember(x => x.PlayStyle, c => c.MapFrom(x => x.PlayStyle ?? PlayStyle.Default(x.Id))).ReverseMap();
        CreateMap<Sidekick, SidekickDto>().ReverseMap();
        CreateMap<Map, MapDto>().ReverseMap();
        CreateMap<Match, MatchDto>().ReverseMap();
        CreateMap<Match, MatchLogDto>()
            .ForMember(x => x.MapName, c => c.MapFrom(x => TryGetMapName(x.Map)))
            .ForMember(x => x.MatchId, c => c.MapFrom(x => x.Id))
            .ForMember(x => x.TournamentName, c => c.MapFrom(x => TryGetTournamentName(x.Tournament, x.Stage)))
            .ForMember(x => x.Fighters, c => c.Ignore())
            .ReverseMap()
            .ForMember(x => x.Map, c => c.Ignore())
            .ForMember(x => x.Tournament, c => c.Ignore());
        CreateMap<Minion, MinionDto>().ReverseMap();
        CreateMap<Fighter, FighterDto>()
            .ForMember(x => x.MatchPoints, c => c.MapFrom(x => x.MatchPoints))
            .ForMember(x => x.SidekickName, c => c.Ignore())
            .ReverseMap()
            .ForMember(x => x.Player, c => c.Ignore())
            .ForMember(x => x.Hero, c => c.Ignore());
        CreateMap<Tournament, TournamentDto>().ReverseMap();
        CreateMap<Title, TitleDto>().ReverseMap();
        CreateMap<Hero, HeroTitleAssignDto>().ForMember(x => x.IsAssigned, c => c.Ignore());
        CreateMap<Villain, VillainDto>().ReverseMap();
        CreateMap<PlayStyle, PlayStyleDto>().ReverseMap();
        CreateMap<Favorite, FavoriteStatisticsDto>()
            .ForMember(x => x.FavoriteId, c => c.MapFrom(x => x.Id))
            .ForMember(x => x.Fights, c => c.Ignore())
            .ForMember(x => x.Place, c => c.Ignore())
            .ForMember(x => x.TotalLooses, c => c.Ignore())
            .ForMember(x => x.TotalMatches, c => c.Ignore())
            .ForMember(x => x.TotalWins, c => c.Ignore())
            .ReverseMap()
            .ForMember(x => x.Player, c => c.Ignore())
            .ForMember(x => x.Hero, c => c.Ignore());
    }

    private string TryGetMapName(Map? map)
    {
        return map?.Name ?? "<forgotten>";
    }

    private string TryGetTournamentName(Tournament? tournament, Stage? stage)
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
