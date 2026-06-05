using RpgGame.Model.Game.Items;

namespace RpgGame.Model.Game;

public interface ITile
{
    bool IsWalkable { get; }
    IReadOnlyList<IItem> Items { get; }

    void AddItem(IItem item);
    bool TryRemoveItem(IItem item);
    IItem? GetTopItem();
    void Accept(ITileVisitor visitor);
}
