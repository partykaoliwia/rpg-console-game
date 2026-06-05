using RpgGame.Model.Game.Items;

namespace RpgGame.Model.Game;

public sealed class FloorTile : ITile
{
    private readonly List<IItem> _items = new();

    public bool IsWalkable => true;
    public IReadOnlyList<IItem> Items => _items;

    public void AddItem(IItem item)
    {
        _items.Add(item);
    }

    public bool TryRemoveItem(IItem item)
    {
        return _items.Remove(item);
    }

    public IItem? GetTopItem()
    {
        return _items.Count == 0 ? null : _items[0];
    }

    public void Accept(ITileVisitor visitor)
    {
        visitor.Visit(this);
    }
}
