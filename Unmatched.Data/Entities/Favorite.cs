namespace Unmatched.Data.Entities;

using System;
using System.ComponentModel.DataAnnotations;

public class Favorite
{
    [Key]
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public Player? Player { get; set; }

    public Guid HeroId { get; set; }

    //public Hero? Hero { get; set; }
    
    public bool IsChosenOne { get; set; }
    
    public int Favour { get; set; }
}
