namespace Unmatched.MatchService.Domain.Models;

public class SaveMatchResult
{
    public string WinnerHeroName { get; set; }
    
    public int WinnerMatchPoints { get; set; }
    
    public string LooserHeroName { get; set; }
    
    public int LooserMatchPoints { get; set; }
    
    public List<string> TitlesEarned { get; set; }
}
