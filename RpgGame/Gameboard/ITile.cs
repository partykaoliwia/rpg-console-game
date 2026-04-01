using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Common;
using RpgGame.Items;

namespace RpgGame.Gameboard
{
    public interface ITile : IDrawable
    {
        bool IsWalkable { get; }
        List<IItem> Items { get; }
        Enemy Entity { get; set; }
    }
}
