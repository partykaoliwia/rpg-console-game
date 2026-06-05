namespace RpgGame.Model.Game.Items;

public sealed class Dory : MagicWeapon
{
    public Dory(int damage)
        : base(damage)
    {
    }

    public override string Name => "Dory";

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
