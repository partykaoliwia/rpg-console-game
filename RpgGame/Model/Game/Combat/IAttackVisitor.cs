using RpgGame.Model.Game.Items;

namespace RpgGame.Model.Game.Combat;

public interface IAttackVisitor
{
    string Name { get; }

    CombatStats Visit(HeavyWeapon weapon, Player player);
    CombatStats Visit(LightWeapon weapon, Player player);
    CombatStats Visit(MagicWeapon weapon, Player player);
    CombatStats Visit(IItem item, Player player);
}
