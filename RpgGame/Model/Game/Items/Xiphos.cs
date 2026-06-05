namespace RpgGame.Model.Game.Items;

public sealed class Xiphos : LightWeapon
{
    public Xiphos(int damage)
        : base(damage)
    {
    }

    public override string Name => "Xiphos";

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
