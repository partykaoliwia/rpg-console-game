namespace RpgGame.Model.Game.Items;

public sealed class Gold : Currency
{
    public const string CurrencyName = "Drachmae";

    public Gold(int value)
        : base(value)
    {
    }

    public override string Name => CurrencyName;

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
