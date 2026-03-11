using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Items
{
    public abstract class Junk : IItem
    {
        public abstract char Symbol { get; }
        public abstract string Name { get; }
        public virtual bool PickUp(Player.Player player)
        {
            return player.TryAddToInventory(this); // Zwróci false, jeśli nie ma miejsca!
        }
        public virtual void EquipToLeftHand(Player.Player player) { }
        public virtual void EquipToRightHand(Player.Player player) { }
    }
}
