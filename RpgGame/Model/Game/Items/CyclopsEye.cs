namespace RpgGame.Model.Game.Items;

public sealed class CyclopsEye : JunkItem
{
    public override string Name => "Cyclops Eye";

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
