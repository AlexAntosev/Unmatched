namespace Unmatched.UI.BlazorServer.Pages.Draft;

using Unmatched.Dtos;
using Unmatched.Enums;

/// <summary>
/// The single source of truth for the 5-step draft session wizard (design/IMPLEMENTATION-PROMPT.md
/// §5A). Lives entirely in Draft.razor's memory - nothing is persisted until step 5's "Save
/// match" (see MatchSheet.SaveAsync); a reload mid-draft loses progress by design (see the plan's
/// decision to keep this fully client-side).
/// </summary>
public class DraftState
{
    public const int Step1Players = 1;
    public const int Step2Map = 2;
    public const int Step3Heroes = 3;
    public const int Step4TurnOrOrVillain = 4;
    public const int Step5Sheet = 5;

    public GameMode Mode { get; set; } = GameMode.OneVsOne;
    public int CurrentStep { get; set; } = Step1Players;
    public int HighestReachedStep { get; set; } = Step1Players;

    public List<UiPlayerDto> Players { get; } = new();
    public Dictionary<Guid, int> Teams { get; } = new();

    public List<MapDto> MapPool { get; set; } = new();
    public List<UiPlayerDto> MapBanOrder { get; set; } = new();
    public Dictionary<Guid, UiPlayerDto> MapBans { get; } = new();
    public MapDto? ChosenMap { get; set; }

    public List<UiHeroDto> HeroPool { get; set; } = new();
    public List<UiPlayerDto> HeroBanOrder { get; set; } = new();
    public Dictionary<Guid, UiPlayerDto> HeroBans { get; } = new();

    /// <summary>Every hero a player has picked in the snake phase (exactly 2 once the phase ends,
    /// before the final-pick step narrows it to one).</summary>
    public Dictionary<Guid, List<UiHeroDto>> PickedHeroesByPlayer { get; } = new();

    /// <summary>The one hero per player that survives the 3.1 final pick.</summary>
    public Dictionary<Guid, Guid> FinalHeroByPlayer { get; } = new();

    public List<(UiPlayerDto Player, int Turn, int? Team)> TurnOrder { get; set; } = new();
    public VillainDto? Villain { get; set; }
    public List<MinionDto> Minions { get; } = new();

    /// <summary>Human-readable ban history for the prefilled sheet's "Draft log" strip.</summary>
    public List<string> BanLog { get; } = new();

    public int MinPlayers => Mode switch
    {
        GameMode.OneVsOne => 2,
        GameMode.TeamVsTeam => 4,
        GameMode.FreeForAll => 3,
        GameMode.Cooperative => 2,
        _ => 2
    };

    public int MaxPlayers => Mode switch
    {
        GameMode.OneVsOne => 2,
        GameMode.TeamVsTeam => 4,
        GameMode.FreeForAll => 4,
        GameMode.Cooperative => 4,
        _ => 2
    };

    public bool IsCoop => Mode == GameMode.Cooperative;

    /// <summary>Whether the current step's choices are complete enough to move on - drives both
    /// the footer's Next-disabled state and the progress rail's "only completed steps are
    /// clickable" rule.</summary>
    public bool CanAdvanceFromStep(int step)
        => step switch
        {
            Step1Players => Players.Count >= MinPlayers && Players.Count <= MaxPlayers
                             && (Mode != GameMode.TeamVsTeam || (Players.Count(p => Teams.GetValueOrDefault(p.Id, 1) == 1) == 2
                                                                 && Players.Count(p => Teams.GetValueOrDefault(p.Id, 1) == 2) == 2)),
            Step2Map => ChosenMap is not null,
            Step3Heroes => FinalHeroByPlayer.Count == Players.Count,
            Step4TurnOrOrVillain => IsCoop ? Villain is not null : TurnOrder.Count == Players.Count,
            _ => true
        };
}
