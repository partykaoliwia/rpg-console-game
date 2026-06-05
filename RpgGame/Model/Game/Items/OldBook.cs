namespace RpgGame.Model.Game.Items;

public sealed class OldBook : JunkItem
{
    public override string Name => "Old Book";

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
