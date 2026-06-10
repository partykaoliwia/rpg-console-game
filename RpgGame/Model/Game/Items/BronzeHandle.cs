namespace RpgGame.Model.Game.Items;

public sealed class BronzeHandle : WeaponHandle
{
    public BronzeHandle(int slotCount = 2)
        : base(slotCount)
    {
    }

    public override string Name => "Bronze Handle";

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
