namespace RpgGame.Model.Game.Items;

public sealed class LuckStone : PassiveItem
{
    public override string Name => "Luck Stone";

    public override PlayerStats Bonuses => new()
    {
        Luck = 2
    };

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
