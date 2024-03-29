﻿namespace Unmatched.Data.Entities;

using System.ComponentModel.DataAnnotations;
using Unmatched.Data.Enums;

public class Tournament
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public TournamentType Type { get; set; }

    public bool IsActive { get; set; } = true;
    
    public Stage InitialStage { get; set; }
    
    public Stage CurrentStage { get; set; }
    
    public virtual ICollection<Match> Matches { get; set; }
}