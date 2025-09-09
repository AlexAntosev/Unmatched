namespace Unmatched.MatchService.Domain.Mapping;

using AutoMapper;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Extensions;

public class UnmatchedMapper : Profile
{
    public UnmatchedMapper()
    {
        CreateMap<>()
    }

    private string TryGetMapName(Map? map)
    {
        return map?.Name ?? "<forgotten>";
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

   
