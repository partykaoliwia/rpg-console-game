using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public abstract class JunkItem : IItem
{
    public abstract string Name { get; }
    public PlayerStats Bonuses => new();
    public int SoundRange => 3;

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

    public void Unequip(Player player)
    {
    }

    public CombatStats AcceptAttack(IAttackVisitor visitor, Player player)
    {
        return visitor.Visit(this, player);
    }

    public abstract void Accept(IItemVisitor visitor);
}
