using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Items;

namespace RpgGame.Gameboard
{
    public class EmptyTile : ITile
    {
        public Color ItemColor => Color.DeepPink;
        public string Symbol => " ";
        public bool IsWalkable => true;
        public List<IItem> Items { get; } = new List<IItem>();

    }
}
