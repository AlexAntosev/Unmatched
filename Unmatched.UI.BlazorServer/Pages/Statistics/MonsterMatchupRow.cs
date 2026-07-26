namespace Unmatched.UI.BlazorServer.Pages.Statistics;

public class MonsterMatchupRow
{
    public string Name { get; set; } = "";

    public int Total { get; set; }

    public int Wins { get; set; }

    public int Looses { get; set; }

    public double Kd
        => Looses > 0
            ? Math.Round((double)Wins / Looses, 2)
            : 0;
}
