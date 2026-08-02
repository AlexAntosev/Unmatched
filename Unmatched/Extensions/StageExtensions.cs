namespace Unmatched.Extensions;

using Unmatched.Enums;

public static class StageExtensions
{
    public static int GetFightersCount(this Stage stage)
    {
        return stage switch
            {
                Stage.Group => 32,
                Stage.SixteenthFinals => 32,
                Stage.EighthFinals => 16,
                Stage.QuarterFinals => 8,
                Stage.SemiFinals => 4,
                Stage.ThirdPlaceDecider => 2,
                Stage.GrandFinals => 2,
                _ => 0
            };
    }
    
    public static string GetStageName(this Stage stage)
    {
        return stage switch
            {
                Stage.Group => "Group Stage",
                Stage.SixteenthFinals => "1/16 Finals",
                Stage.EighthFinals => "1/8 Finals",
                Stage.QuarterFinals => "1/4 Finals",
                Stage.SemiFinals => "1/2 Finals",
                Stage.ThirdPlaceDecider => "3rd Place Decider",
                Stage.GrandFinals => "Grand Final",
                _ => ""
            };
    }
}
