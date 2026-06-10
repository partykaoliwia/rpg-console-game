using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public abstract class ItemDecorator : IItem
{
    private readonly IItem _decorated;

    protected ItemDecorator(IItem decorated)
    {
        _decorated = decorated;
    }

    protected IItem Decorated => _decorated;
    public IItem InnerItem => _decorated;
    public virtual string Name => _decorated.Name;
    public virtual PlayerStats Bonuses => _decorated.Bonuses;
    public virtual int SoundRange => _decorated.SoundRange;
    public virtual IReadOnlyList<ItemSlot> Slots => _decorated.Slots;
    public virtual bool CanBePlacedInWeaponSlot => _decorated.CanBePlacedInWeaponSlot;
    public virtual bool CanBePlacedInHandleSlot => _decorated.CanBePlacedInHandleSlot;

    public virtual bool PickUp(Player player)
    {
        return player.TryAddToInventory(this);
    }

    public virtual bool EquipToLeftHand(Player player, IItem? instanceToEquip = null)
    {
        return _decorated.EquipToLeftHand(player, instanceToEquip ?? this);
    }

    public virtual bool EquipToRightHand(Player player, IItem? instanceToEquip = null)
    {
        return _decorated.EquipToRightHand(player, instanceToEquip ?? this);
    }

    public virtual bool TryPlaceInSlot(int slotIndex, IItem item)
    {
        return _decorated.TryPlaceInSlot(slotIndex, item);
    }

    public virtual void Unequip(Player player)
    {
        _decorated.Unequip(player);
    }

    public virtual CombatStats AcceptAttack(IAttackVisitor visitor, Player player)
    {
        return _decorated.AcceptAttack(visitor, player);
    }

    public virtual void Accept(IItemVisitor visitor)
    {
        _decorated.Accept(visitor);
    }
}
