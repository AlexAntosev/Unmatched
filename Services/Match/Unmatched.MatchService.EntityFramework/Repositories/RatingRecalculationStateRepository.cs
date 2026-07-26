namespace Unmatched.MatchService.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.EntityFramework.Context;

public class RatingRecalculationStateRepository(UnmatchedDbContext dbContext) : IRatingRecalculationStateRepository
{
    // there is only ever one row in this table, so a fixed id avoids needing a separate "get the row" lookup key.
    private static readonly Guid StateId = new("9a4f2b13-6b1e-4a2b-9b1a-8e6b6c2f2b13");

    public async Task<bool> IsRecalculationRequiredAsync()
    {
        var state = await dbContext.RatingRecalculationStates.AsNoTracking().FirstOrDefaultAsync(s => s.Id == StateId);
        return state?.IsRecalculationRequired ?? false;
    }

    public async Task SetRecalculationRequiredAsync(bool isRequired)
    {
        var state = await dbContext.RatingRecalculationStates.FirstOrDefaultAsync(s => s.Id == StateId);
        if (state is null)
        {
            dbContext.RatingRecalculationStates.Add(new RatingRecalculationStateEntity { Id = StateId, IsRecalculationRequired = isRequired });
        }
        else
        {
            state.IsRecalculationRequired = isRequired;
        }

        await dbContext.SaveChangesAsync();
    }
}
