namespace RpgGame.Model.Game.Items;

public sealed class AphroditesKissModifier : ItemDecorator
{
    public AphroditesKissModifier(IItem decorated)
        : base(decorated)
    {
    }

    public override string Name => $"{Decorated.Name} (Kissed by Aphrodite)";

    public override PlayerStats Bonuses => Decorated.Bonuses + new PlayerStats
    {
        Health = 20,
        Aggression = -15
    };

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
