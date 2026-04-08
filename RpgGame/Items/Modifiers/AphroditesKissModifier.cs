using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Modifiers
{
    internal class AphroditesKissModifier : ItemDecorator
    {
        // Health +20
        // Aggression -15
        private int health = 20;
        private int aggression = -15;
        public AphroditesKissModifier(IItem decorated) : base(decorated) { }
        public override string Name => _decorated.Name + "(Kissed by Aphrodite)";

        public override void EquipToLeftHand(Player player, IItem instanceToEquip = null)
        {
            base.EquipToLeftHand(player, instanceToEquip ?? this);
            player.Health += health;
            player.Aggression += aggression;
        }
        public override void EquipToRightHand(Player player, IItem instanceToEquip = null)
        {
            base.EquipToRightHand(player, instanceToEquip ?? this);
            player.Health += health;
            player.Aggression += aggression;

        }
        public override void Unequip(Player player)
        {
            base.Unequip(player);
            player.Health -= health;
            player.Aggression -= aggression;

        }
        public override CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            return base.Accept(visitor, player);
        }
    }
}
