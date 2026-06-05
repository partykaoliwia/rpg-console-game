using RpgGame.Model.Game.Items;

namespace RpgGame.Model.Game.Combat;

public sealed class NormalAttack : IAttackVisitor
{
    public string Name => "normal attack";

    public CombatStats Visit(HeavyWeapon weapon, Player player)
    {
        return new CombatStats
        {
            Damage = weapon.GetFullDamage(player),
            Defense = player.Luck + player.Strength
        };
    }

    public CombatStats Visit(LightWeapon weapon, Player player)
    {
        return new CombatStats
        {
            Damage = weapon.GetFullDamage(player),
            Defense = player.Luck + player.Dexterity
        };
    }

    public CombatStats Visit(MagicWeapon weapon, Player player)
    {
        return new CombatStats
        {
            Damage = 1,
            Defense = player.Luck + player.Dexterity
        };
    }

    public CombatStats Visit(IItem item, Player player)
    {
        return new CombatStats
        {
            Damage = 0,
            Defense = player.Dexterity
        };
    }
}
