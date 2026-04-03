using RpgGame.Characters;
using RpgGame.Items;
using RpgGame.Items.Weapons;

namespace RpgGame.Combat
{
    public interface IAttackVisitor
    {
        CombatStats Visit(HeavyWeapon weapon, Player player);
        CombatStats Visit(LightWeapon weapon, Player player);
        CombatStats Visit(MagicWeapon weapon, Player player);
        CombatStats Visit(IItem item, Player player);
    }
}