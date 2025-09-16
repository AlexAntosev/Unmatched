namespace Unmatched.MatchService.Api.Dto;

public class SaveMatchResultDto
{
    public string WinnerHeroName { get; set; }
    
    public int WinnerMatchPoints { get; set; }
    
    public string LooserHeroName { get; set; }
    
    public int LooserMatchPoints { get; set; }
    
    public List<string> TitlesEarned { get; set; }
}
