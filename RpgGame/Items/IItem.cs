using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Player;

namespace RpgGame.Items
{
    public interface IItem
    {
        char Symbol { get; }
        string Name { get; }
        void PickUp(Player.Player player);
        void EquipToLeftHand(Player.Player player);
        void EquipToRightHand(Player.Player player);
        //void Unequip(Player.Player player);
        //void Equip(Player.Player player);
    }
}
