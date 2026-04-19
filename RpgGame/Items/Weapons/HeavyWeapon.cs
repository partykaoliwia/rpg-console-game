using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Weapons
{
    public abstract class HeavyWeapon : Weapon
    {
        public HeavyWeapon(int damage) : base(damage) { }
        public override int GetFullDamage(Player player) => Damage + player.Strength + player.Aggression;
        public override CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            return visitor.Visit(this, player);
        }
    }
}
