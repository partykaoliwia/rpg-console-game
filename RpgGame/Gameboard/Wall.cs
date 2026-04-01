using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Common;
using RpgGame.Items;

namespace RpgGame.Gameboard
{
    public class Wall : ITile
    {
        public Color ItemColor => Color.Bisque;
        public string Symbol => "█";
        public bool IsWalkable => false;
        public List<IItem> Items { get; } = new List<IItem>();

    }
}
