namespace Unmatched.MatchService.Domain.Dto;

using Unmatched.MatchService.Domain.Dto.Catalog;
using Unmatched.MatchService.Domain.Entities;

class MatchContextDto
{
    public MatchContextDto(
        CatalogHeroDto winnerReferenceHero,
        CatalogHeroDto looserReferenceHero,
        Fighter winnerFighter,
        Fighter looserFighter,
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

    public Fighter LooserFighter { get; }

    public int LooserPointsBeforeMatch { get; }

    public CatalogHeroDto LooserReferenceHero { get; }

    public Fighter WinnerFighter { get; }

    public int WinnerPointsBeforeMatch { get; }

    public CatalogHeroDto WinnerReferenceHero { get; }
}
