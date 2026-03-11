using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Items
{
    public abstract class Weapon : IItem
    {
        public abstract char Symbol { get; }
        public abstract string Name { get; }
        public int Damage { get; set; }
        protected Weapon(int damage)
        {
            Damage = damage;
        }
        public virtual bool PickUp(Player.Player player)
        {
            return player.TryAddToInventory(this); // Zwróci false, jeśli nie ma miejsca!
        }
        public virtual void EquipToLeftHand(Player.Player player)
        {
            player.EquipToLeftHand(this);
        }
        public virtual void EquipToRightHand(Player.Player player)
        {
            player.EquipToRightHand(this);
        }
    }
}
