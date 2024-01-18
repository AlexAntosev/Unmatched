namespace Unmatched.UI.BlazorServer;

using System;

using Unmatched.Dtos;

public class NotificationService
{
    public event Action<FighterDto, FighterDto?> OnRandomizeFighter = delegate { };
    public void CallRandomizeFighter(FighterDto fighter, FighterDto? exceptFighter = null) => OnRandomizeFighter.Invoke(fighter, exceptFighter);
    
    public event Action RefreshRequested;
    public void CallRequestRefresh()
    {
        RefreshRequested?.Invoke();
    }
}
