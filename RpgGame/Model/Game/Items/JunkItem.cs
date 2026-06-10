using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public abstract class JunkItem : IItem
{
    public abstract string Name { get; }
    public virtual PlayerStats Bonuses => new();
    public virtual int SoundRange => 3;
    public virtual IReadOnlyList<ItemSlot> Slots => Array.Empty<ItemSlot>();
    public virtual bool CanBePlacedInWeaponSlot => false;
    public virtual bool CanBePlacedInHandleSlot => false;

    public virtual bool PickUp(Player player)
    {
        return player.TryAddToInventory(this);
    }

    public virtual bool EquipToLeftHand(Player player, IItem? instanceToEquip = null)
    {
        return false;
    }

    public virtual bool EquipToRightHand(Player player, IItem? instanceToEquip = null)
    {
        return false;
    }

    public virtual bool TryPlaceInSlot(int slotIndex, IItem item)
    {
        return false;
    }

    public virtual void Unequip(Player player)
    {
    }

    public virtual CombatStats AcceptAttack(IAttackVisitor visitor, Player player)
    {
        return visitor.Visit(this, player);
    }

    public abstract void Accept(IItemVisitor visitor);
}
