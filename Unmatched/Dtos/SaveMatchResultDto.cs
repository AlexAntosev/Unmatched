namespace Unmatched.Dtos;

public class SaveMatchResultDto
{
    public string WinnerHeroName { get; set; }
    
    public int WinnerMatchPoints { get; set; }
    
    public string LooserHeroName { get; set; }
    
    public int LooserMatchPoints { get; set; }
    
    public List<TitleDto> TitlesEarned { get; set; }
}
