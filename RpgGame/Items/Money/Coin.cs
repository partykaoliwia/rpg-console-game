using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;

namespace RpgGame.Items.Money
{
    public class Coin : Currency
    {
        public override string Symbol => "©";
        public override string Name => CurrencyName;
        private readonly int _value;
        public static string CurrencyName = "Charon's Obols";

        public Coin(int value) : base(value) { }
        public override bool PickUp(Player player)
        {
            player.AddCurrency(Name, Value);
            return true;
        }

    }
}
