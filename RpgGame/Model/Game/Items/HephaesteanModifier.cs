using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public sealed class HephaesteanModifier : ItemDecorator
{
    public HephaesteanModifier(IItem decorated)
        : base(decorated)
    {
    }

    public override string Name => $"{Decorated.Name} (Hephaestean)";

    public override PlayerStats Bonuses => Decorated.Bonuses + new PlayerStats
    {
        Dexterity = -15
    };

    public override CombatStats AcceptAttack(IAttackVisitor visitor, Player player)
    {
        var stats = Decorated.AcceptAttack(visitor, player);
        return new CombatStats
        {
            Damage = (int)Math.Round(stats.Damage * 1.5),
            Defense = stats.Defense
        };
    }

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
