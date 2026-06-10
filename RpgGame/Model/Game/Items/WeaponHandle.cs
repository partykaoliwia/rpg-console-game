using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public abstract class WeaponHandle : IItem
{
    private readonly ItemSlotCollection _slots;

    protected WeaponHandle(int slotCount)
    {
        _slots = new ItemSlotCollection(slotCount, ItemSlotKind.Handle);
    }

    public abstract string Name { get; }
    public virtual PlayerStats OwnBonuses => new();
    public PlayerStats Bonuses => OwnBonuses + _slots.Bonuses;
    public int SoundRange => 3;
    public IReadOnlyList<ItemSlot> Slots => _slots.Slots;
    public bool CanBePlacedInWeaponSlot => true;
    public bool CanBePlacedInHandleSlot => true;

    public bool PickUp(Player player)
    {
        return player.TryAddToInventory(this);
    }

    public bool EquipToLeftHand(Player player, IItem? instanceToEquip = null)
    {
        return false;
    }

    public bool EquipToRightHand(Player player, IItem? instanceToEquip = null)
    {
        return false;
    }

    public bool TryPlaceInSlot(int slotIndex, IItem item)
    {
        return _slots.TryPlaceInSlot(slotIndex, item);
    }

    public void Unequip(Player player)
    {
    }

    public CombatStats AcceptAttack(IAttackVisitor visitor, Player player)
    {
        return visitor.Visit(this, player);
    }

    public abstract void Accept(IItemVisitor visitor);
}
