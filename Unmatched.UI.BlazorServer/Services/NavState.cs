namespace Unmatched.UI.BlazorServer.Services;

/// <summary>
/// Whether the nav rail is expanded. Scoped to the Blazor circuit so the choice survives
/// navigation between screens, which is what the handoff means by "persists across screens".
/// </summary>
public class NavState
{
    private bool _isExpanded = true;

    public event Action? Changed;

    public bool IsExpanded => _isExpanded;

    public void Toggle()
    {
        _isExpanded = !_isExpanded;
        Changed?.Invoke();
    }
}
