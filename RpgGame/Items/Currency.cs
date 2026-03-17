using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;

namespace RpgGame.Items
{
    public abstract class Currency : IItem
    {
        public abstract char Symbol { get; }
        public abstract string Name { get; }
        public int Value { get; protected set; }

        protected Currency(int value)
        {
            Value = value;
        }
        public abstract bool PickUp(Player player);
        public void EquipToLeftHand(Player player) { }
        public void EquipToRightHand(Player player) { }
    }
}
