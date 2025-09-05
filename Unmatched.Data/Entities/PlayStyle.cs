// namespace Unmatched.Data.Entities;
//
// using System;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
//
// public class PlayStyle
// {
//     [Key]
//     public Guid Id { get; set; }
//     
//     [ForeignKey(nameof(Hero.Id))]
//     public Guid HeroId { get; set; }
//     
//     public int Attack { get; set; }
//     
//     public int Defence { get; set; }
//     
//     public int Trickery { get; set; }
//     
//     public int Difficulty { get; set; }
//     
//     public static PlayStyle Default(Guid heroId)
//     {
//         return new PlayStyle()
//             {
//                 HeroId = heroId,
//                 Id = Guid.NewGuid()
//             };
//     }
// }
