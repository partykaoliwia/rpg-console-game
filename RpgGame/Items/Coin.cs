using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Items
{
    public class Coin : IItem
    {
        public char Symbol => 'o';
        public string Name => "Coin";
        private readonly int _value;
        public Coin(int value) { _value = value;}
        public void PickUp(Player.Player player)
        {
            player.AddCoins(_value);
        }
        public void EquipToLeftHand(Player.Player player) { }
        public void EquipToRightHand(Player.Player player) { }

    }
}
