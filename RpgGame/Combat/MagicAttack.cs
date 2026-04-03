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
    public class MagicAttack : IAttackVisitor
    {
        public CombatStats Visit(HeavyWeapon weapon, Player player)
        {
            return new CombatStats
            {
                Damage = 1,
                Defense = player.Luck,
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
                Damage = weapon.Damage,
                Defense = player.Wisdom*2
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
}
