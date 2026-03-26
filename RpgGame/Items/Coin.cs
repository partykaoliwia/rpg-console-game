using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;

namespace RpgGame.Items
{
    public class Coin : Currency
    {
        public override char Symbol => 'O';
        public override string Name => CurrencyName;
        private readonly int _value;
        public static string CurrencyName = "Obol Charona";

        public Coin(int value) : base(value) { }
        public override bool PickUp(Player player)
        {
            player.AddCurrency(this.Name, this.Value);
            return true;
        }

    }
}
