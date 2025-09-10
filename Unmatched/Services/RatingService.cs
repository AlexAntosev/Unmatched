using Unmatched.Services.Contracts;

namespace Unmatched.Services;

using Unmatched.HttpClients.Contracts;

public class RatingService(IMatchClient client) : IRatingService
{
    public Task RecalculateAsync()
    {
        return client.RecalculateAsync();
    }
}
