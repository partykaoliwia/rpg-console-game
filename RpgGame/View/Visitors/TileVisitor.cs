using RpgGame.Model.Game;

namespace RpgGame.View.Visitors;

public sealed class TileVisitor : ITileVisitor
{
    public RenderIcon Icon { get; private set; } = new("?", ConsoleColor.DarkGray);

    public void Visit(FloorTile floor)
    {
        Icon = new RenderIcon(" ", ConsoleColor.DarkGray);
    }

    public void Visit(WallTile wall)
    {
        Icon = new RenderIcon("█", ConsoleColor.DarkYellow);
    }
}
