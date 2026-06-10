namespace RpgGame.Model.Game.Items;

public sealed class ItemSlot
{
    public ItemSlot(ItemSlotKind kind)
    {
        Kind = kind;
    }

    public ItemSlotKind Kind { get; }
    public IItem? Item { get; private set; }

    public bool TryPlace(IItem item)
    {
        if (Item != null || !CanAccept(item))
        {
            return false;
        }

        Item = item;
        return true;
    }

    private bool CanAccept(IItem item)
    {
        if (Kind == ItemSlotKind.Weapon)
        {
            return item.CanBePlacedInWeaponSlot;
        }

        return item.CanBePlacedInHandleSlot;
    }
}
