namespace Unmatched.Mapping;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.Dtos.Catalog;
using Unmatched.Dtos.Match;
using Unmatched.Dtos.Player;
using Unmatched.Dtos.Statistics;

public class UnmatchedMapper : Profile
{
    public UnmatchedMapper()
    {
          CreateMap<CatalogHeroDto, UiHeroDto>()
              .ForMember(x => x.ImageUrl, c => c.Ignore())
              .ForMember(x => x.MeleeRangeImageUrl, c => c.Ignore())
              .ReverseMap();
          CreateMap<CatalogSidekickDto, UiSidekickDto>()
              .ForMember(x => x.MeleeRangeImageUrl, c => c.Ignore());
          CreateMap<FighterDto, UiFighterDto>().ReverseMap();
          CreateMap<FighterHeroDto, UiHeroDto>()
              .ForMember(x => x.ImageUrl, c => c.Ignore())
              .ForMember(x => x.MeleeRangeImageUrl, c => c.Ignore()).ReverseMap();
          CreateMap<FighterSidekickDto, UiSidekickDto>()
              .ForMember(x => x.MeleeRangeImageUrl, c => c.Ignore()).ReverseMap();
          CreateMap<MatchDto, UiMatchDto>()
              .ReverseMap();
          CreateMap<MatchLogDto, UiMatchLogDto>().ReverseMap();
          CreateMap<PlayerDto, UiPlayerDto>().ReverseMap();

          CreateMap<CatalogMapDto, MapDto>()
              .ForMember(x => x.ImageUrl, c => c.Ignore())
              .ReverseMap();

          CreateMap<MapDto, MatchMapDto>()
              .ReverseMap();

          CreateMap<HeroStatisticsDto, UiHeroStatisticsDto>();
    }
}
