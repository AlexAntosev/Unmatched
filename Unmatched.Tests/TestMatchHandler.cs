namespace Unmatched.Tests;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.Services.MatchHandlers;

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