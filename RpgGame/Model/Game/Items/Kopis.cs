namespace RpgGame.Model.Game.Items;

public sealed class Kopis : HeavyWeapon
{
    public Kopis(int damage)
        : base(damage)
    {
    }

    public override string Name => "Kopis";

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
