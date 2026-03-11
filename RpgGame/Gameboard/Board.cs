using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Gameboard
{
    public class Board
    {
        private readonly ITile[,] _tiles;
        public int Width { get; } = 40;
        public int Height { get; } = 20;
        private readonly string[] _mapBlueprint = new string[]
        {
            " #######################################",
            "                                       #",
            "#                                      #",
            "#                                      #",
            "#         ##                ##         #",
            "#         ##                ##         #",
            "#         ##                ##         #",
            "#                                      #",
            "#                                      #",
            "#                                      #",
            "#                                      #",
            "#   #                              #   #",
            "#    #                            #    #",
            "#     ##                        ##     #",
            "#       ###                  ###       #",
            "#          ##################          #",
            "#                                      #",
            "#                                      #",
            "#                                      #",
            "########################################"
        };
        public Board()
        {
            _tiles = new ITile[Width, Height];
            InitializeBoard();
        }
        public void InitializeBoard()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    char symbol = _mapBlueprint[y][x];
                    if(symbol=='#')
                    {
                        _tiles[x, y] = new Wall();
                    }
                    else
                    {
                        _tiles[x, y] = new EmptyTile();
                    }
                }

            }
        }
        public ITile GetTile(int x, int y)
        {
            // jesli wyjdziemy poza mape
            if (x < 0 || x >= Width || y < 0 || y >= Height) return new Wall();

            return _tiles[x, y];
        }
        public void DrawBoard()
        {
            Console.SetCursorPosition(0, 0);
            for(int y=0; y<Height;y++)
            {
                for(int x=0; x<Width; x++)
                {
                    Console.Write(_tiles[x, y].Symbol);
                }
                Console.WriteLine();
            }
        }

    }
}
