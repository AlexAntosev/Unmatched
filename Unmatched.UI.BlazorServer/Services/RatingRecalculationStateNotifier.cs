namespace Unmatched.UI.BlazorServer.Services;

public class RatingRecalculationStateNotifier
{
    public event Action? Changed;

    public void NotifyChanged() => Changed?.Invoke();
}
