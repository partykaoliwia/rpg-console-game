using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Items
{
    public class Gold : Currency
    {
        public override char Symbol => 'G';
        public override string Name => "Gold";

        public Gold(int value) : base(value) { }

        public override bool PickUp(Player.Player player)
        {
            player.AddGold(Value);
            return true;
        }
    }
}
