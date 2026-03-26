using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;

namespace RpgGame.Items
{
    public abstract class Weapon : IItem
    {
        public Color ItemColor => Color.DarkSeaGreen;
        public abstract string Symbol { get; }
        public abstract string Name { get; }
        public int Damage { get; set; }


        protected Weapon(int damage)
        {
            Damage = damage;
        }
        public virtual bool PickUp(Player player)
        {
            return player.TryAddToInventory(this);
        }
        public virtual void EquipToLeftHand(Player player)
        {
            player.EquipToLeftHand(this);
        }
        public virtual void EquipToRightHand(Player player)
        {
            player.EquipToRightHand(this);
        }
    }
}
