namespace RpgGame.Model.Game.Items;

public sealed class ShatteredShield : JunkItem
{
    public override string Name => "Shattered Shield";

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
