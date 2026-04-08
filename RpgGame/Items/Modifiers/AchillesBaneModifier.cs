using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;
using static System.Net.Mime.MediaTypeNames;

namespace RpgGame.Items.Modifiers
{
    internal class AchillesBaneModifier : ItemDecorator
    {
        // Defense +100%
        // Luck -15
        private int luck = -15;
        private double defense = 2;
        public AchillesBaneModifier(IItem decorated) : base(decorated) { }
        public override string Name => _decorated.Name + "(Achilles bane)";

        public override void EquipToLeftHand(Player player, IItem instanceToEquip = null)
        {
            base.EquipToLeftHand(player, instanceToEquip ?? this);
            player.Luck += luck;
        }
        public override void EquipToRightHand(Player player, IItem instanceToEquip = null)
        {
            base.EquipToRightHand(player, instanceToEquip ?? this);
            player.Luck += luck;

        }
        public override void Unequip(Player player)
        {
            base.Unequip(player);
            player.Luck -= luck;

        }
        public override CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            var stats = base.Accept(visitor, player);
            stats.Defense = (int)Math.Round(stats.Defense * defense);
            return stats;
        }
    }
}
