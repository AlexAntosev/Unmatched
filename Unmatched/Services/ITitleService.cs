namespace Unmatched.Services;

using Unmatched.Dtos;

public interface ITitleService
{
    Task AddAsync(TitleDto title);
}
