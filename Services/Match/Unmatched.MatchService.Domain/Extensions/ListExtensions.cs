namespace Unmatched.MatchService.Domain.Extensions;

using System.Text.Json;

public static class ListExtensions
{
    public static List<T> Clone<T>(this List<T> list)
    {
        var clone = JsonSerializer.Deserialize<List<T>>(JsonSerializer.Serialize(list));
        return clone!;
    }
    
    public static List<T> Shuffle<T>(this List<T> list)
    {
        var random = new Random();
        var shuffled = new List<T>();

        var listCopy = list.Clone();
        var listCount = listCopy.Count;
        for (var i = 0; i < listCount; i++)
        {
            var randomIndex = random.Next(0, listCopy.Count);
            shuffled.Add(listCopy[randomIndex]);
            listCopy.Remove(listCopy[randomIndex]);
        }
        
        return shuffled;
    }
    
    public static T GetAndRemoveRandomItem<T>(this List<T> list)
    {
        var index = new Random().Next(0, list.Count);
        var item = list.ToArray()[index];
        list.Remove(item);

        return item;
    }
    
    public static T GetRandomItem<T>(this List<T> list)
    {
        var index = new Random().Next(0, list.Count);
        var item = list.ToArray()[index];

        return item;
    }
}
