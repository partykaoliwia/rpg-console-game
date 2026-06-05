using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public abstract class Weapon : IItem
{
    protected Weapon(int damage)
    {
        Damage = damage;
    }

    public int Damage { get; }
    public abstract string Name { get; }
    public virtual PlayerStats Bonuses => new();
    public virtual int SoundRange => 6;

    public abstract int GetFullDamage(Player player);

    public virtual bool PickUp(Player player)
    {
        return player.TryAddToInventory(this);
    }

    public virtual bool EquipToLeftHand(Player player, IItem? instanceToEquip = null)
    {
        return player.EquipToLeftHand(instanceToEquip ?? this);
    }

    public virtual bool EquipToRightHand(Player player, IItem? instanceToEquip = null)
    {
        return player.EquipToRightHand(instanceToEquip ?? this);
    }

    public virtual void Unequip(Player player)
    {
    }

    public abstract CombatStats AcceptAttack(IAttackVisitor visitor, Player player);
    public abstract void Accept(IItemVisitor visitor);
}
