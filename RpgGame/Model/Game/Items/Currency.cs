using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public abstract class Currency : IItem
{
    protected Currency(int value)
    {
        Value = value;
    }

    public int Value { get; }
    public abstract string Name { get; }
    public PlayerStats Bonuses => new();
    public int SoundRange => 4;
    public IReadOnlyList<ItemSlot> Slots => Array.Empty<ItemSlot>();
    public bool CanBePlacedInWeaponSlot => false;
    public bool CanBePlacedInHandleSlot => false;

    public bool PickUp(Player player)
    {
        player.AddCurrency(Name, Value);
        return true;
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
        return false;
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
