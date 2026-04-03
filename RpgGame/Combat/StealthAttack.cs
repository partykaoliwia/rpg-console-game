using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Items;
using RpgGame.Items.Weapons;

namespace RpgGame.Combat
{
    public class StealthAttack : IAttackVisitor
    {
        public CombatStats Visit(HeavyWeapon weapon, Player player)
        {
            return new CombatStats
            {
                Damage = weapon.Damage/2, // zredukowane do połowy
                Defense = player.Strength // siła
            };
        }

        public CombatStats Visit(LightWeapon weapon, Player player)
        {
            return new CombatStats
            {
                Damage = weapon.Damage*2, // podwojone
                Defense = player.Dexterity // zręczność
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
}
