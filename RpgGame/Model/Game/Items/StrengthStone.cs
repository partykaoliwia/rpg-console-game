namespace RpgGame.Model.Game.Items;

public sealed class StrengthStone : PassiveItem
{
    public override string Name => "Strength Stone";

    public override PlayerStats Bonuses => new()
    {
        Strength = 2
    };

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
