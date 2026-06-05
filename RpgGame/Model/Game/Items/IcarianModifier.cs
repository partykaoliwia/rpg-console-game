using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public sealed class IcarianModifier : ItemDecorator
{
    public IcarianModifier(IItem decorated)
        : base(decorated)
    {
    }

    public override string Name => $"{Decorated.Name} (Icarian)";

    public override PlayerStats Bonuses => Decorated.Bonuses + new PlayerStats
    {
        Dexterity = 10,
        Luck = -10
    };

    public override CombatStats AcceptAttack(IAttackVisitor visitor, Player player)
    {
        var stats = Decorated.AcceptAttack(visitor, player);
        return new CombatStats
        {
            Damage = (int)Math.Round(stats.Damage * 1.3),
            Defense = (int)Math.Round(stats.Defense * 0.85)
        };
    }

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
