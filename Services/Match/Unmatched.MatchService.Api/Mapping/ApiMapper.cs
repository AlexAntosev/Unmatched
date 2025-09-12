namespace Unmatched.MatchService.Api.Mapping;

using AutoMapper;

using Unmatched.MatchService.Api.Dto;
using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Dto.Catalog;

public class ApiMapper : Profile
{
    public ApiMapper()
    {
        CreateMap<Match, MatchDto>().ReverseMap();
        CreateMap<MatchLog, MatchLogDto>().ReverseMap();
        CreateMap<FighterHero, FighterHeroDto>().ReverseMap();
        CreateMap<FighterSidekick, FighterSidekickDto>().ReverseMap();
        CreateMap<Fighter, FighterDto>().ReverseMap();
        CreateMap<Tournament, TournamentDto>().ReverseMap();
        CreateMap<HeroTitleAssign, HeroTitleAssignDto>().ReverseMap();
        CreateMap<SaveMatchResult, SaveMatchResultDto>().ReverseMap();
        CreateMap<CatalogMapDto, MapDto>().ReverseMap();
    }
}

   
