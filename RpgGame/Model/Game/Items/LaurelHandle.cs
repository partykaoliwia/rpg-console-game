namespace RpgGame.Model.Game.Items;

public sealed class LaurelHandle : WeaponHandle
{
    public LaurelHandle(int slotCount = 2)
        : base(slotCount)
    {
    }

    public override string Name => "Laurel Handle";

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
