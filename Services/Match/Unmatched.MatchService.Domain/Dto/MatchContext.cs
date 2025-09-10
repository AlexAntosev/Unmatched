namespace Unmatched.MatchService.Domain.Dto;

using Unmatched.MatchService.Domain.Dto.Catalog;
using Unmatched.MatchService.Domain.Entities;

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
