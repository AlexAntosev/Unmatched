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
        CreateMap<Hero, HeroDto>().ReverseMap();
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
        CreateMap<Fighter, FighterDto>()
            .ForMember(x => x.PlayerName, c => c.MapFrom(x => x.Player.Name))
            .ForMember(x => x.HeroName, c => c.MapFrom(x => x.Hero.Name))
            .ForMember(x => x.HeroColor, c => c.MapFrom(x => x.Hero.Color))
            .ForMember(x => x.MatchPoints, c => c.MapFrom(x => x.MatchPoints))
            .ReverseMap()
            .ForMember(x => x.Player, c => c.Ignore())
            .ForMember(x => x.Hero, c => c.Ignore());
        CreateMap<Tournament, TournamentDto>().ReverseMap();
        CreateMap<Title, TitleDto>().ReverseMap();
        CreateMap<Hero, HeroTitleAssignDto>();
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
