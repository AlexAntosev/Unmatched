namespace Unmatched.Initializer.Utils;

using System.Text.Json;

public static class JsonReader
{
    public static List<T> ReadJson<T>(string path)
    {
        var jsonString = File.ReadAllText(path);
        var obj = JsonSerializer.Deserialize<List<T>>(jsonString);
        if (obj is null)
        {
            throw new InvalidDataException("Json is invalid");
        }
        return obj;
    }
}
