namespace RpgGame.Model.Game.Items;

public sealed class WisdomStone : PassiveItem
{
    public override string Name => "Wisdom Stone";

    public override PlayerStats Bonuses => new()
    {
        Wisdom = 2
    };

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
