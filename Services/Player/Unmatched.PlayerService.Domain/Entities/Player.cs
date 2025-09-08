namespace Unmatched.PlayerService.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class Player
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }
}
