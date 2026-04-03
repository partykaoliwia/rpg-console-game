using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Money
{
    public abstract class Currency : IItem
    {
        public Color ItemColor { get; set; } = Color.Gold;
        public abstract string Symbol { get; }
        public abstract string Name { get; }
        public int Value { get; protected set; }


        protected Currency(int value)
        {
            Value = value;
        }
        public abstract bool PickUp(Player player);
        public void EquipToLeftHand(Player player) { }
        public void EquipToRightHand(Player player) { }
        public virtual void Unequip(Player player) { }
        public virtual CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            return visitor.Visit(this, player);
        }
    }
}
