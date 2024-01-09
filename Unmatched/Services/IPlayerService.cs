namespace Unmatched.Services;

using Unmatched.Dtos;

public interface IPlayerService
{
    Task<IEnumerable<PlayerDto>> GetAsync();
}
