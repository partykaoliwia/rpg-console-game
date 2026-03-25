using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace RpgGame.Gameboard
{
    public class Board
    {
        private readonly ITile[,] _tiles;
        public int Width { get; } = 40;
        public int Height { get; } = 20;
        public Board()
        {
            _tiles = new ITile[Width, Height];
        }
        public void SetTile(int x, int y, ITile tile)
        {
            if(x>=0 && x < Width && y >= 0 && y < Height)
            {
                _tiles[x, y] = tile;
            }
        }
        public ITile GetTile(int x, int y)
        {
            // jesli wyjdziemy poza mape
            if (x < 0 || x >= Width || y < 0 || y >= Height) return new Wall();
            return _tiles[x, y];
        }
    }
}
