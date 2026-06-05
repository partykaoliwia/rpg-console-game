namespace RpgGame.Model.Game.Items;

public sealed class IcarusFeather : JunkItem
{
    public override string Name => "Icarus Feather";

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
