namespace RpgGame.Model.Game.Items;

public abstract class PassiveItem : JunkItem
{
    public override bool CanBePlacedInWeaponSlot => true;
    public override bool CanBePlacedInHandleSlot => true;

    public override bool EquipToLeftHand(Player player, IItem? instanceToEquip = null)
    {
        return player.EquipToLeftHand(instanceToEquip ?? this);
    }

    public override bool EquipToRightHand(Player player, IItem? instanceToEquip = null)
    {
        return player.EquipToRightHand(instanceToEquip ?? this);
    }
}
