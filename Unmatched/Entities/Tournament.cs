﻿namespace Unmatched.Entities;

using System.ComponentModel.DataAnnotations;

public class Tournament
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public TournamentType Type { get; set; }
}

public enum TournamentType
{
    League,

    Championship
}
