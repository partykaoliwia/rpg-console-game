using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;

namespace RpgGame.Items.Money
{
    public class Gold : Currency
    {
        public override string Symbol => "¤";
        public override string Name => CurrencyName;
        public static string CurrencyName = "Złota Drachma";

        public Gold(int value) : base(value) { }

        public override bool PickUp(Player player)
        {
            player.AddCurrency(Name, Value);
            return true;
        }
    }
}
