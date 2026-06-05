namespace RpgGame.Model.Game.Items;

public sealed class StymphalianQuill : LightWeapon
{
    public StymphalianQuill(int damage)
        : base(damage)
    {
    }

    public override string Name => "Stymphalian Quill";

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
