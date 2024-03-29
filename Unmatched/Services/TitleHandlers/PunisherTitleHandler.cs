﻿namespace Unmatched.Services.TitleHandlers;

using AutoMapper;

using Unmatched.Constants;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.Dtos;

public class PunisherTitleHandler : IPunisherTitleHandler
{
    private const double MinVictoryPointsForTitle = 1000;
    
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PunisherTitleHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<TitleDto?> HandleAsync(Match match)
    {
        var title = await _unitOfWork.Titles.GetByNameAsync(Titles.Punisher);
        if (title is null)
        {
            return null;
        }

        var winner = match.Fighters.FirstOrDefault(f => f.IsWinner);

        if (winner is not null)
        {
            var isAlreadyPunisher = title.Heroes.Any(h => h.Id == winner.HeroId);
            if (!isAlreadyPunisher && winner.MatchPoints >= MinVictoryPointsForTitle)
            {
                title.Heroes.Add(winner.Hero);
                _unitOfWork.Titles.AddOrUpdate(title);
                await _unitOfWork.SaveChangesAsync();

                var titleDto = _mapper.Map<TitleDto>(title);
                return titleDto;
            }
        }

        return null;
    }
}
