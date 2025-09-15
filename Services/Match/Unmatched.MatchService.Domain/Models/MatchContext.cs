namespace Unmatched.MatchService.Domain.Models;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Models.Catalog;

class MatchContext
{
    public MatchContext(
        CatalogHeroDto winnerReferenceHero,
        CatalogHeroDto looserReferenceHero,
        FighterEntity winnerFighter,
        FighterEntity looserFighter,
        int winnerPointsBeforeMatch,
        int looserPointsBeforeMatch)
    {
        WinnerReferenceHero = winnerReferenceHero;
        LooserReferenceHero = looserReferenceHero;
        WinnerFighter = winnerFighter;
        LooserFighter = looserFighter;
        WinnerPointsBeforeMatch = winnerPointsBeforeMatch;
        LooserPointsBeforeMatch = looserPointsBeforeMatch;
    }

    public FighterEntity LooserFighter { get; }

    public int LooserPointsBeforeMatch { get; }

    public CatalogHeroDto LooserReferenceHero { get; }

    public FighterEntity WinnerFighter { get; }

    public int WinnerPointsBeforeMatch { get; }

    public CatalogHeroDto WinnerReferenceHero { get; }
}
