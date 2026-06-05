using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public sealed class AchillesBaneModifier : ItemDecorator
{
    public AchillesBaneModifier(IItem decorated)
        : base(decorated)
    {
    }

    public override string Name => $"{Decorated.Name} (Achilles bane)";

    public override PlayerStats Bonuses => Decorated.Bonuses + new PlayerStats
    {
        Luck = -15
    };

    public override CombatStats AcceptAttack(IAttackVisitor visitor, Player player)
    {
        var stats = Decorated.AcceptAttack(visitor, player);
        return new CombatStats
        {
            Damage = stats.Damage,
            Defense = (int)Math.Round(stats.Defense * 2.0)
        };
    }

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
