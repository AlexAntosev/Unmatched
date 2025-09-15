namespace Unmatched.MatchService.Api.Mapping;

using AutoMapper;

using Unmatched.MatchService.Api.Dto;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Models;

public class ApiMapper : Profile
{
    public ApiMapper()
    {
        CreateMap<Match, MatchDto>().ReverseMap();
        CreateMap<MatchLog, MatchLogDto>().ReverseMap();
        CreateMap<FighterHero, FighterHeroDto>().ReverseMap();
        CreateMap<FighterSidekick, FighterSidekickDto>().ReverseMap();
        CreateMap<Fighter, FighterDto>().ReverseMap();
        CreateMap<Fighter, HeroStatsFighterDto>();
        CreateMap<Tournament, TournamentDto>().ReverseMap();
        CreateMap<HeroTitleAssign, HeroTitleAssignDto>().ReverseMap();
        CreateMap<SaveMatchResult, SaveMatchResultDto>().ReverseMap();
        CreateMap<CatalogMapDto, MapDto>().ReverseMap();
        CreateMap<FighterPlayer, FighterPlayerDto>().ReverseMap();
        CreateMap<Rating, RatingDto>();
        CreateMap<RatingChange, RatingChangeDto>();
    }
}

   
