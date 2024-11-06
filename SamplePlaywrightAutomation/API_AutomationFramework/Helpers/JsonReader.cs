using System.Text.Json;

namespace API_AutomationFramework.Helpers
{
    public class JsonFileReader
    {
        public static T ReadJson<T>(string filepath)
        {
            string JsonString = File.ReadAllText(filepath);
            return JsonSerializer.Deserialize<T>(JsonString)!;

        }
    }
}
