namespace Unmatched.Services;

using System;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.Entities;
using Unmatched.Services.MatchHandlers;

public class MatchService : IMatchService
{
    private readonly IMatchHandlerFactory _matchHandlerFactory;

    private readonly IMapper _mapper;

    public MatchService(IMatchHandlerFactory matchHandlerFactory, IMapper mapper)
    {
        _matchHandlerFactory = matchHandlerFactory;
        _mapper = mapper;
    }
    public async Task AddAsync(Match match)
    {
        var handler = _matchHandlerFactory.Create(match);
        await handler.HandleAsync(match);
    }

    public async Task AddAsync(MatchDto matchDto, FighterDto fighterDto, FighterDto opponentDto)
    {
        var match = _mapper.Map<Match>(matchDto);
        var firstFighter = _mapper.Map<Fighter>(fighterDto);
        var secondFighter = _mapper.Map<Fighter>(opponentDto);

        match.Fighters = new List<Fighter>()
            {
                firstFighter,
                secondFighter
            };

        await AddAsync(match);
    }
}
