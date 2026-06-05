namespace RpgGame.Model.Game.Items;

public sealed class Coin : Currency
{
    public const string CurrencyName = "Coin";

    public Coin(int value)
        : base(value)
    {
    }

    public override string Name => CurrencyName;

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
