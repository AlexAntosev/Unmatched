namespace Unmatched.Mapping;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.Dtos.Catalog;
using Unmatched.Dtos.Match;

public class UnmatchedMapper : Profile
{
    public UnmatchedMapper()
    {
          CreateMap<CatalogHeroDto, UiHeroDto>()
              .ForMember(x => x.ImageUrl, c => c.Ignore())
              .ForMember(x => x.MeleeRangeImageUrl, c => c.Ignore())
              .ReverseMap();
          CreateMap<FighterDto, UiFighterDto>().ReverseMap();
          CreateMap<FighterHeroDto, UiHeroDto>()
              .ForMember(x => x.ImageUrl, c => c.Ignore())
              .ForMember(x => x.MeleeRangeImageUrl, c => c.Ignore()).ReverseMap();
          CreateMap<FighterSidekickDto, UiSidekickDto>()
              .ForMember(x => x.MeleeRangeImageUrl, c => c.Ignore()).ReverseMap();
          CreateMap<MatchDto, UiMatchDto>().ReverseMap();
          CreateMap<MatchLogDto, UiMatchLogDto>().ReverseMap();

          CreateMap<CatalogMapDto, MapDto>()
              .ForMember(x => x.ImageUrl, c => c.Ignore())
              .ReverseMap();
          //  CreateMap<Player, PlayerDto>().ReverseMap();
        // // CreateMap<Sidekick, SidekickDto>().ReverseMap();
        //  CreateMap<CatalogHeroDto, HeroDto>()
        //      .ForMember(x => x.Sidekicks, c => c.Ignore())
        //      .ForMember(x => x.Titles, c => c.Ignore())
        //      .ForMember(x => x.PlayStyle, c => c.Ignore())
        //      .ReverseMap();
        //  CreateMap<CatalogMapDto, MapDto>().ReverseMap();
        //  //CreateMap<Match, MatchDto>().ReverseMap();
        // // CreateMap<Match, MatchLogDto>()
        //      // .ForMember(x => x.MapName, c => c.MapFrom(x => TryGetMapName(x.Map)))
        //      // .ForMember(x => x.MatchId, c => c.MapFrom(x => x.Id))
        //      // .ForMember(x => x.TournamentName, c => c.MapFrom(x => TryGetTournamentName(x.Tournament, x.Stage)))
        //      // .ForMember(x => x.Fighters, c => c.Ignore())
        //      // .ReverseMap()
        //      // .ForMember(x => x.Map, c => c.Ignore())
        //      // .ForMember(x => x.Tournament, c => c.Ignore());
        //  CreateMap<Minion, MinionDto>().ReverseMap();
        //  CreateMap<Fighter, FighterDto>()
        //      .ForMember(x => x.MatchPoints, c => c.MapFrom(x => x.MatchPoints))
        //      .ForMember(x => x.SidekickName, c => c.Ignore())
        //      .ReverseMap()
        //      .ForMember(x => x.Player, c => c.Ignore());
        //  CreateMap<Tournament, TournamentDto>().ReverseMap();
        //  CreateMap<Title, TitleDto>().ReverseMap();
        //  //CreateMap<Hero, HeroTitleAssignDto>().ForMember(x => x.IsAssigned, c => c.Ignore());
        //  CreateMap<Villain, VillainDto>().ReverseMap();
        //  CreateMap<PlayStyle, PlayStyleDto>().ReverseMap();
        //  CreateMap<Favorite, FavoriteStatisticsDto>()
        //      .ForMember(x => x.FavoriteId, c => c.MapFrom(x => x.Id))
        //      .ForMember(x => x.Fights, c => c.Ignore())
        //      .ForMember(x => x.Place, c => c.Ignore())
        //      .ForMember(x => x.TotalLooses, c => c.Ignore())
        //      .ForMember(x => x.TotalMatches, c => c.Ignore())
        //      .ForMember(x => x.TotalWins, c => c.Ignore())
        //      .ReverseMap()
        //      .ForMember(x => x.Player, c => c.Ignore());
    }
}
