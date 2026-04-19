using RpgGame.Characters;
using RpgGame.Items;
using RpgGame.Items.Weapons;

namespace RpgGame.Combat
{
    public class NormalAttack : IAttackVisitor
    {
        public CombatStats Visit(HeavyWeapon weapon, Player player)
        {
            return new CombatStats
            {
                Damage = weapon.GetFullDamage(player),
                Defense = player.Luck + player.Strength // siła + szczęście
            };
        }

        public CombatStats Visit(LightWeapon weapon, Player player)
        {
            return new CombatStats
            {
                Damage = weapon.GetFullDamage(player),
                Defense = player.Luck + player.Dexterity // zręczność + szczęście
            }; 
        }

        public CombatStats Visit(MagicWeapon weapon, Player player)
        {
            return new CombatStats
            {
                Damage = 1,
                Defense = player.Luck + player.Dexterity // zręczność + szczęście
            };
        }

        public CombatStats Visit(IItem item, Player player)
        {
            // atak czymś innym niż broń
            return new CombatStats
            {
                Damage = 0,
                Defense = player.Dexterity // zręczność
            };
        }
    }
}