using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public interface IItem
{
    string Name { get; }
    PlayerStats Bonuses { get; }
    int SoundRange { get; }
    IReadOnlyList<ItemSlot> Slots { get; }
    bool CanBePlacedInWeaponSlot { get; }
    bool CanBePlacedInHandleSlot { get; }

    bool PickUp(Player player);
    bool EquipToLeftHand(Player player, IItem? instanceToEquip = null);
    bool EquipToRightHand(Player player, IItem? instanceToEquip = null);
    bool TryPlaceInSlot(int slotIndex, IItem item);
    void Unequip(Player player);
    CombatStats AcceptAttack(IAttackVisitor visitor, Player player);
    void Accept(IItemVisitor visitor);
}
