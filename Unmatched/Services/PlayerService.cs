using Unmatched.Services.Contracts;

namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.Dtos.Player;
using Unmatched.HttpClients.Contracts;

public class PlayerService(IPlayerClient playerClient, IMapper mapper) : IPlayerService
{
    public async Task<IEnumerable<UiPlayerDto>> GetAsync()
    {
        var players = await playerClient.GetAllAsync();
        return players.Select(mapper.Map<UiPlayerDto>);
    }

    public async Task AddAsync(UiPlayerDto dto)
    {
        var playerDto = mapper.Map<PlayerDto>(dto);
        await playerClient.AddAsync(playerDto);
    }
}
