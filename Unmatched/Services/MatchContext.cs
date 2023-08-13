using Unmatched.Entities;

namespace Unmatched.Services;

class MatchContext
{
    public MatchContext(Hero winnerReferenceHero, Hero looserReferenceHero, Fighter winnerFighter,
        Fighter looserFighter, int winnerPointsBeforeMatch, int looserPointsBeforeMatch)
    {
        WinnerReferenceHero = winnerReferenceHero;
        LooserReferenceHero = looserReferenceHero;
        WinnerFighter = winnerFighter;
        LooserFighter = looserFighter;
        WinnerPointsBeforeMatch = winnerPointsBeforeMatch;
        LooserPointsBeforeMatch = looserPointsBeforeMatch;
    }

    public Hero WinnerReferenceHero { get;  }
    public Hero LooserReferenceHero { get; }
    public Fighter WinnerFighter { get;  }
    public Fighter LooserFighter { get;  }
    public int WinnerPointsBeforeMatch { get;  }
    public int LooserPointsBeforeMatch { get;  }
}