namespace Unmatched.Services;

using Unmatched.Entities;

class MatchContext
{
    public MatchContext(
        Hero winnerReferenceHero,
        Hero looserReferenceHero,
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

    public Hero LooserReferenceHero { get; }

    public Fighter WinnerFighter { get; }

    public int WinnerPointsBeforeMatch { get; }

    public Hero WinnerReferenceHero { get; }
}
