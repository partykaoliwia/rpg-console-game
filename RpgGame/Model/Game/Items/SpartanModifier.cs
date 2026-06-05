namespace RpgGame.Model.Game.Items;

public sealed class SpartanModifier : ItemDecorator
{
    public SpartanModifier(IItem decorated)
        : base(decorated)
    {
    }

    public override string Name => $"{Decorated.Name} (Spartan)";

    public override PlayerStats Bonuses => Decorated.Bonuses + new PlayerStats
    {
        Strength = 10,
        Luck = -5
    };

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
