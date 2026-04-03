using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Modifiers
{
    internal class DionysianModifier : ItemDecorator
    {
        // Wisdom -10
        private int wisdom = -10;
        public DionysianModifier(IItem decorated) : base(decorated) { }
        public override string Name => _decorated.Name + "(Dionysian)";

        public override void EquipToLeftHand(Player player)
        {
            base.EquipToLeftHand(player);
            player.Wisdom += wisdom;
        }
        public override void EquipToRightHand(Player player)
        {
            base.EquipToRightHand(player);
            player.Wisdom += wisdom;

        }
        public override void Unequip(Player player)
        {
            base.Unequip(player);
            player.Wisdom -= wisdom;

        }
        public override CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            return base.Accept(visitor,player);
        }
    }
}
