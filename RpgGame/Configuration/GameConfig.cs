namespace RpgGame.Configuration;

public sealed class GameConfig
{
    public string PlayerName { get; set; } = "Tezeusz";
    public string ClientPlayerName { get; set; } = "Hero";
    public string DungeonTheme { get; set; } = "Random";
    public int DefaultPort { get; set; } = 5555;
    public string DefaultHost { get; set; } = "127.0.0.1";
    public string KeybindsPath { get; set; } = "keybinds.txt";
    public string LogDirectory { get; set; } = "GameLogs";
}
