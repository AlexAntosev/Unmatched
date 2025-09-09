namespace Unmatched.UI.BlazorServer;

using System;
using Unmatched.Dtos;
using Unmatched.Dtos.Match;

public class NotificationService
{
    public event Action<UiFighterDto, UiFighterDto?> OnRandomizeFighter = delegate { };
    public void CallRandomizeFighter(UiFighterDto fighter, UiFighterDto? exceptFighter = null) => OnRandomizeFighter.Invoke(fighter, exceptFighter);
    
    public event Action RefreshRequested;
    public void CallRequestRefresh()
    {
        RefreshRequested?.Invoke();
    }
}
