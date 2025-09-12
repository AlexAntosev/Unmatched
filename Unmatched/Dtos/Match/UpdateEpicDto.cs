namespace Unmatched.Dtos.Match;

public class UpdateEpicDto
{
    public UpdateEpicDto(int epicValue)
    {
        EpicValue = epicValue;
    }

    public UpdateEpicDto()
    {
    }

    public int EpicValue { get; set; }
}
