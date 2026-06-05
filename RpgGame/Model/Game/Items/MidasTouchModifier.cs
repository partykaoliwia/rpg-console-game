using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public sealed class MidasTouchModifier : ItemDecorator
{
    public MidasTouchModifier(IItem decorated)
        : base(decorated)
    {
    }

    public override string Name => $"{Decorated.Name} (Midas touch)";

    public override PlayerStats Bonuses => Decorated.Bonuses + new PlayerStats
    {
        Luck = 20
    };

    public override CombatStats AcceptAttack(IAttackVisitor visitor, Player player)
    {
        var stats = Decorated.AcceptAttack(visitor, player);
        return new CombatStats
        {
            Damage = (int)Math.Round(stats.Damage * 0.75),
            Defense = stats.Defense
        };
    }

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
