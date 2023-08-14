using System.Linq.Expressions;
using AutoMapper;
using Unmatched.Dtos;
using Unmatched.Entities;

namespace Unmatched.Mapping;

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
            .ForMember(x => x.MapName, c => c.MapFrom(x => x.Map.Name))
            .ForMember(x => x.MatchId, c => c.MapFrom(x => x.Id))
            .ForMember(x => x.TournamentName, c => c.MapFrom(x => TryGetTournamentName(x.Tournament)))
            .ForMember(x => x.Fighters, c => c.Ignore())
            .ReverseMap();
        CreateMap<Fighter, FighterDto>().ReverseMap();
        CreateMap<Tournament, TournamentDto>().ReverseMap();
        CreateMap<Rating, RatingDto>().ReverseMap();
    }

    private string TryGetTournamentName(Tournament tournament)
    {
        return tournament?.Name ?? string.Empty;
    }
}