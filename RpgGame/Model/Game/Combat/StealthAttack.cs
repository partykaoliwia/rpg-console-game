using RpgGame.Model.Game.Items;

namespace RpgGame.Model.Game.Combat;

public sealed class StealthAttack : IAttackVisitor
{
    public string Name => "stealth attack";

    public CombatStats Visit(HeavyWeapon weapon, Player player)
    {
        return new CombatStats
        {
            Damage = weapon.GetFullDamage(player) / 2,
            Defense = player.Strength
        };
    }

    public CombatStats Visit(LightWeapon weapon, Player player)
    {
        return new CombatStats
        {
            Damage = weapon.GetFullDamage(player) * 2,
            Defense = player.Dexterity
        };
    }

    public CombatStats Visit(MagicWeapon weapon, Player player)
    {
        return new CombatStats
        {
            Damage = 1,
            Defense = 0
        };
    }

    public CombatStats Visit(IItem item, Player player)
    {
        return new CombatStats
        {
            Damage = 0,
            Defense = 0
        };
    }
}
