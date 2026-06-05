using RpgGame.Model.Game.Items;

namespace RpgGame.Model.Game.Combat;

public sealed class MagicAttack : IAttackVisitor
{
    public string Name => "magic attack";

    public CombatStats Visit(HeavyWeapon weapon, Player player)
    {
        return new CombatStats
        {
            Damage = 1,
            Defense = player.Luck
        };
    }

    public CombatStats Visit(LightWeapon weapon, Player player)
    {
        return new CombatStats
        {
            Damage = 1,
            Defense = player.Luck
        };
    }

    public CombatStats Visit(MagicWeapon weapon, Player player)
    {
        return new CombatStats
        {
            Damage = weapon.GetFullDamage(player),
            Defense = player.Wisdom * 2
        };
    }

    public CombatStats Visit(IItem item, Player player)
    {
        return new CombatStats
        {
            Damage = 0,
            Defense = player.Luck
        };
    }
}
