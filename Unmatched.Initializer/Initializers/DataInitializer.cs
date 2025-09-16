// namespace Unmatched.Initializer.Initializers;
//
// using Unmatched.Data.Entities;
// using Unmatched.Data.Repositories;
// using Unmatched.Initializer.Utils;
//
// public class DataInitializer : IDataInitializer
// {
//     private readonly IUnitOfWork _unitOfWork;
//
//     public DataInitializer(IUnitOfWork unitOfWork)
//     {
//         _unitOfWork = unitOfWork;
//     }
//     
//     // public async Task InitializeHeroesAsync()
//     // {
//     //     var heroes = JsonReader.ReadJson<Hero>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "heroes.json"));
//     //
//     //     await _unitOfWork.Heroes.AddRangeAsync(heroes);
//     //     await _unitOfWork.SaveChangesAsync();
//     // }
//     
//     public async Task InitializeMapsAsync()
//     {
//         var maps = JsonReader.ReadJson<Map>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "maps.json"));
//
//         await _unitOfWork.Maps.AddRangeAsync(maps);
//         await _unitOfWork.SaveChangesAsync();
//     }
//     
//     public async Task InitializeMinionsAsync()
//     {
//         var minions = JsonReader.ReadJson<Minion>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "minions.json"));
//
//         await _unitOfWork.Minions.AddRangeAsync(minions);
//         await _unitOfWork.SaveChangesAsync();
//     }
//     
//     public async Task InitializeVillainsAsync()
//     {
//         var villains = JsonReader.ReadJson<Villain>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "villains.json"));
//
//         await _unitOfWork.Villains.AddRangeAsync(villains);
//         await _unitOfWork.SaveChangesAsync();
//     }
// }
