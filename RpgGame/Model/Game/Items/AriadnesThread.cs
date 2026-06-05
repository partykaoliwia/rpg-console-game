namespace RpgGame.Model.Game.Items;

public sealed class AriadnesThread : JunkItem
{
    public override string Name => "Ariadne's Thread";

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
