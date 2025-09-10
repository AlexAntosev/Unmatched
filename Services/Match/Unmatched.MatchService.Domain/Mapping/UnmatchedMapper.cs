namespace Unmatched.MatchService.Domain.Mapping;

using AutoMapper;

using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Extensions;

public class UnmatchedMapper : Profile
{
    public UnmatchedMapper()
    {
        CreateMap<MatchEntity, Match>().ReverseMap();
        CreateMap<MatchEntity, MatchLog>().ReverseMap();
        CreateMap<FighterEntity, Fighter>().ReverseMap();
        CreateMap<TitleEntity, Title>().ReverseMap();
        CreateMap<TournamentEntity, Tournament>().ReverseMap();
    }

    //private string TryGetMapName(Map? map)
    //{
    //    return map?.Name ?? "<forgotten>";
    //}

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

   
