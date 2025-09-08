namespace Unmatched.MatchService.Tests;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.Repositories;

public class TestMatchHandler : BaseMatchHandler
{
    protected override async Task InnerHandleAsync(Match match)
    {
    }

    protected override void InnerValidate(Match match)
    {
    }

    public TestMatchHandler(IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }
}