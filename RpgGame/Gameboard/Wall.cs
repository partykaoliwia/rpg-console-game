using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Items;

namespace RpgGame.Gameboard
{
    public class Wall : ITile
    {
        public char Symbol => '█';
        public bool IsWalkable => false;
        public List<IItem> Items { get; } = new List<IItem>();
    }
}
