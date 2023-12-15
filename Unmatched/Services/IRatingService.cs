namespace Unmatched.Services;

using System;

public interface IRatingService
{
    public Task RecalculateAsync();
}
