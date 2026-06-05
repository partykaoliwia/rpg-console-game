using RpgGame.Model.Game;

namespace RpgGame.View.Visitors;

public sealed class CharacterVisitor : ICharacterVisitor
{
    private static readonly IReadOnlyDictionary<string, RenderIcon> NamedEnemyIcons =
        new Dictionary<string, RenderIcon>(StringComparer.OrdinalIgnoreCase)
        {
            { "Minotaur", new RenderIcon("M", ConsoleColor.DarkYellow) },
            { "Deimos", new RenderIcon("Δ", ConsoleColor.Red) },
            { "Medusa", new RenderIcon("M", ConsoleColor.Green) },
            { "Champion of Ares", new RenderIcon("A", ConsoleColor.DarkRed) },
            { "Guardian of Alexandria", new RenderIcon("G", ConsoleColor.DarkGreen) },
            { "Ancient Giant", new RenderIcon("G", ConsoleColor.DarkMagenta) },
            { "Arena Satyr", new RenderIcon("s", ConsoleColor.DarkYellow) },
            { "Archivist Satyr", new RenderIcon("s", ConsoleColor.DarkCyan) },
            { "Lost Satyr", new RenderIcon("s", ConsoleColor.DarkGreen) }
        };

    private readonly int _localPlayerId;

    public CharacterVisitor(int localPlayerId)
    {
        _localPlayerId = localPlayerId;
    }

    public RenderIcon Icon { get; private set; } = new("?", ConsoleColor.White);

    public void Visit(Player player)
    {
        var color = player.Id == _localPlayerId ? ConsoleColor.White : ConsoleColor.Cyan;
        Icon = new RenderIcon(player.Id.ToString(), color);
    }

    public void Visit(Harpy harpy)
    {
        Icon = CreateEnemyIcon(harpy.Name, new RenderIcon("h", ConsoleColor.Magenta));
    }

    public void Visit(Giant giant)
    {
        Icon = CreateEnemyIcon(giant.Name, new RenderIcon("G", ConsoleColor.Red));
    }

    public void Visit(Satyr satyr)
    {
        Icon = CreateEnemyIcon(satyr.Name, new RenderIcon("s", ConsoleColor.DarkYellow));
    }

    private static RenderIcon CreateEnemyIcon(string enemyName, RenderIcon defaultIcon)
    {
        return NamedEnemyIcons.TryGetValue(enemyName, out var icon) ? icon : defaultIcon;
    }
}
