using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Unusables
{
    public abstract class Junk : IItem
    {
        public abstract string Symbol { get; }
        public virtual PlayerStats Bonuses => new PlayerStats();

        public abstract string Name { get; }

        public abstract Color ItemColor { get; set; }

        public virtual bool PickUp(Player player)
        {
            return player.TryAddToInventory(this);
        }
        public virtual void EquipToLeftHand(Player player, IItem instanceToEquip = null) { }
        public virtual void EquipToRightHand(Player player, IItem instanceToEquip = null) { }
        public virtual void Unequip(Player player) { }
        public virtual CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            return visitor.Visit(this, player);
        }
    }
}
