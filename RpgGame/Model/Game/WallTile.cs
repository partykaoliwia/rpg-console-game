using RpgGame.Model.Game.Items;

namespace RpgGame.Model.Game;

public sealed class WallTile : ITile
{
    public static readonly WallTile OutsideBoard = new();

    public bool IsWalkable => false;
    public IReadOnlyList<IItem> Items => Array.Empty<IItem>();

    public void AddItem(IItem item)
    {
    }

    public bool TryRemoveItem(IItem item)
    {
        return false;
    }

    public IItem? GetTopItem()
    {
        return null;
    }

    public void Accept(ITileVisitor visitor)
    {
        visitor.Visit(this);
    }
}
