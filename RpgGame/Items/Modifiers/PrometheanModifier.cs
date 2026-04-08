using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Modifiers
{
    internal class PrometheanModifier : ItemDecorator
    {
        // Wisdom +15
        // Health -10
        private int wisdom = 15;
        private int health = -10;
        public PrometheanModifier(IItem decorated) : base(decorated) { }
        public override string Name => _decorated.Name + "(Promethean)";

        public override void EquipToLeftHand(Player player, IItem instanceToEquip = null)
        {
            base.EquipToLeftHand(player, instanceToEquip ?? this);
            player.Wisdom += wisdom;
            player.Health += health;
        }
        public override void EquipToRightHand(Player player, IItem instanceToEquip = null)
        {
            base.EquipToRightHand(player, instanceToEquip ?? this);
            player.Wisdom += wisdom;
            player.Health += health;

        }
        public override void Unequip(Player player)
        {
            base.Unequip(player);
            player.Wisdom -= wisdom;
            player.Health -= health;

        }
        public override CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            return base.Accept(visitor,player);
        }
    }
}
