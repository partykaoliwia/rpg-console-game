using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public sealed class AthenianModifier : ItemDecorator
{
    public AthenianModifier(IItem decorated)
        : base(decorated)
    {
    }

    public override string Name => $"{Decorated.Name} (Athenian)";

    public override PlayerStats Bonuses => Decorated.Bonuses + new PlayerStats
    {
        Wisdom = 15
    };

    public override CombatStats AcceptAttack(IAttackVisitor visitor, Player player)
    {
        var stats = Decorated.AcceptAttack(visitor, player);
        return new CombatStats
        {
            Damage = (int)Math.Round(stats.Damage * 1.2),
            Defense = stats.Defense
        };
    }

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
