namespace RpgGame.Model.Game.Items;

public sealed class DionysianModifier : ItemDecorator
{
    public DionysianModifier(IItem decorated)
        : base(decorated)
    {
    }

    public override string Name => $"{Decorated.Name} (Dionysian)";

    public override PlayerStats Bonuses => Decorated.Bonuses + new PlayerStats
    {
        Wisdom = -10
    };

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
