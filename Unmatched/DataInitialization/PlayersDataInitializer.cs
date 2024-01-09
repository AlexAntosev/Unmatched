namespace Unmatched.DataInitialization;

using Unmatched.Constants;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;

public class PlayersDataInitializer : IDataInitializer<Player>
{
    private readonly IPlayerRepository _playerRepository;

    public PlayersDataInitializer(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public IEnumerable<Player> GetEntities()
    {
        return new List<Player>
            {
                new()
                    {
                        Name = PlayerNames.Andrii
                    },
                new()
                    {
                        Name = PlayerNames.Oleksandr
                    },
                new()
                    {
                        Name = PlayerNames.Ksuha
                    },
                
                new()
                    {
                        Name = PlayerNames.Tetyana
                    },
            };
    }

    public async Task InitializeAsync()
    {
        var defaultPlayers = GetEntities();

        await _playerRepository.AddRangeAsync(defaultPlayers);
        await _playerRepository.SaveChangesAsync();
    }
}
