using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;

namespace RpgGame.Items
{
    public abstract class Junk : IItem
    {
        public abstract string Symbol { get; }
        public abstract string Name { get; }

        public abstract Color ItemColor { get; set; }

        public virtual bool PickUp(Player player)
        {
            return player.TryAddToInventory(this);
        }
        public virtual void EquipToLeftHand(Player player) { }
        public virtual void EquipToRightHand(Player player) { }
    }
}
