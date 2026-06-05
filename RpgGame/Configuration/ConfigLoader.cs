using System.Text.Json;

namespace RpgGame.Configuration;

public static class ConfigLoader
{
    public static GameConfig LoadOrDefault(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new GameConfig();
        }

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<GameConfig>(json) ?? new GameConfig();
    }
}
