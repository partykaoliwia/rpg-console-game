using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public interface IItem
{
    string Name { get; }
    PlayerStats Bonuses { get; }
    int SoundRange { get; }

    bool PickUp(Player player);
    bool EquipToLeftHand(Player player, IItem? instanceToEquip = null);
    bool EquipToRightHand(Player player, IItem? instanceToEquip = null);
    void Unequip(Player player);
    CombatStats AcceptAttack(IAttackVisitor visitor, Player player);
    void Accept(IItemVisitor visitor);
}
