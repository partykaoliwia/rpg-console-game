namespace RpgGame.Model.Game.Items;

public sealed class Labrys : HeavyWeapon
{
    public Labrys(int damage)
        : base(damage)
    {
    }

    public override string Name => "Labrys";

    public override bool EquipToLeftHand(Player player, IItem? instanceToEquip = null)
    {
        return player.EquipToBothHands(instanceToEquip ?? this);
    }

    public override bool EquipToRightHand(Player player, IItem? instanceToEquip = null)
    {
        return player.EquipToBothHands(instanceToEquip ?? this);
    }

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
