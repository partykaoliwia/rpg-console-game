using RpgGame.Model.Game;
using RpgGame.Model.Game.Items;

namespace RpgGame.Model.Local;

public static class SlotBrowserNavigator
{
    public static IItem? GetRootItem(Player? player, LocalModel localModel)
    {
        if (player == null ||
            localModel.SlotRootInventoryIndex < 0 ||
            localModel.SlotRootInventoryIndex >= player.Inventory.Count)
        {
            return null;
        }

        return player.Inventory[localModel.SlotRootInventoryIndex];
    }

    public static IItem? GetCurrentItem(Player? player, LocalModel localModel)
    {
        return GetCurrentItem(GetRootItem(player, localModel), localModel.SlotPath);
    }

    public static IItem? GetCurrentItem(IItem? rootItem, IReadOnlyList<int> slotPath)
    {
        var currentItem = rootItem;
        foreach (var slotIndex in slotPath)
        {
            if (currentItem == null ||
                slotIndex < 0 ||
                slotIndex >= currentItem.Slots.Count)
            {
                return null;
            }

            currentItem = currentItem.Slots[slotIndex].Item;
        }

        return currentItem;
    }
}
