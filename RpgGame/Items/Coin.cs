using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Items
{
    public class Coin : Currency
    {
        public override char Symbol => 'o';
        public override string Name => "Coin";
        private readonly int _value;
        public Coin(int value) : base(value) { }
        public override bool PickUp(Player.Player player)
        {
            player.AddCoins(Value);
            return true; // Zawsze można wziąć pieniądze
        }

    }
}
