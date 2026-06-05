namespace RpgGame.Model.Game.Items;

public sealed class PrometheanModifier : ItemDecorator
{
    public PrometheanModifier(IItem decorated)
        : base(decorated)
    {
    }

    public override string Name => $"{Decorated.Name} (Promethean)";

    public override PlayerStats Bonuses => Decorated.Bonuses + new PlayerStats
    {
        Wisdom = 15,
        Health = -10
    };

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
