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
        CreateMap<Fighter, FighterDto>().ReverseMap();
        CreateMap<Tournament, TournamentDto>().ReverseMap();
        CreateMap<Rating, RatingDto>().ReverseMap();
    }
}