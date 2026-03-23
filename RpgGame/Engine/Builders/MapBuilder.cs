using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Gameboard;

namespace RpgGame.Engine.Builders
{
    public class MapBuilder : IDungeonBuilder
    {
        private Board _board;
        private Random _random = new Random();
        public MapBuilder()
        {
            _board = new Board();
        }
        public Board BuildResult() => _board;
        public void BuildEmptyDungeon()
        {
            for (int y = 0; y < _board.Height; y++)
            {
                for (int x = 0; x < _board.Width; x++)
                {
                    _board.SetTile(x, y, new EmptyTile());
                }
            }
        }

        public void BuildFilledDungeon()
        {
            for (int y = 0; y < _board.Height; y++)
            {
                for (int x = 0; x < _board.Width; x++)
                {
                    _board.SetTile(x, y, new Wall());
                }
            }
        }

        public void AddCorridors()
        {
            // Algorytm "Pijanego Minotaura" (Random Walk)
            // Startujemy ze środka mapy
            int currentX = _board.Width / 2;
            int currentY = _board.Height / 2;

            // Długość korytarza w kaflach (np. 150 kroków)
            int steps = 150;

            for (int i = 0; i < steps; i++)
            {
                // Wyryj korytarz w obecnym miejscu
                _board.SetTile(currentX, currentY, new EmptyTile());

                // Wybierz losowy kierunek: 0-Góra, 1-Prawo, 2-Dół, 3-Lewo
                int direction = _random.Next(0, 4);

                if (direction == 0 && currentY > 1) currentY--;
                else if (direction == 1 && currentX < _board.Width - 2) currentX++;
                else if (direction == 2 && currentY < _board.Height - 2) currentY++;
                else if (direction == 3 && currentX > 1) currentX--;
            }
        }

        public void AddRooms()
        {
            // Losujemy od 3 do 6 komnat na mapie
            int roomCount = _random.Next(3, 7);

            for (int i = 0; i < roomCount; i++)
            {
                int roomWidth = _random.Next(3, 8); // szerokość pokoju: 3-7
                int roomHeight = _random.Next(3, 8); // wysokość pokoju: 3-7

                // Losujemy pozycję, tak aby nie wyjść poza tablicę (zostawiamy 1 kafelek marginesu)
                int startX = _random.Next(1, _board.Width - roomWidth - 1);
                int startY = _random.Next(1, _board.Height - roomHeight - 1);

                for (int y = startY; y < startY + roomHeight; y++)
                {
                    for (int x = startX; x < startX + roomWidth; x++)
                    {
                        _board.SetTile(x, y, new EmptyTile());
                    }
                }
            }
        }

        public void AddCentralHall(int width, int height)
        {
            width = Math.Min(width, _board.Width - 2);
            height = Math.Min(height, _board.Height - 2);

            int startX = (_board.Width - width) / 2;
            int startY = (_board.Height - height) / 2;

            for (int y = startY; y < startY + height; y++)
            {
                for (int x = startX; x < startX + width; x++)
                {
                    _board.SetTile(x, y, new EmptyTile());
                }
            }
        }

        public void AddItems(int count)
        {
            int placed = 0;
            while (placed < count)
            {
                int x = _random.Next(0, _board.Width);
                int y = _random.Next(0, _board.Height);

                var tile = _board.GetTile(x, y);

                // Kładziemy item TYLKO na podłodze (nie na ścianie)
                if (tile.IsWalkable)
                {
                    // Przykład dodania itemu (musisz mieć jakieś zaimplementowane klasy)
                    // tile.Items.Add(new AriadnesThread());

                    placed++;
                }
            }
        }

        public void AddWeapons(int count)
        {
            int placed = 0;

            while (placed < count)
            {
                int x = _random.Next(0, _board.Width);
                int y = _random.Next(0, _board.Height);

                var tile = _board.GetTile(x, y);

                if (tile.IsWalkable)
                {
                    // Losujemy typ broni
                    if (_random.Next(0, 2) == 0)
                    {
                        // tile.Items.Add(new XiphosSword());
                    }
                    else
                    {
                        // tile.Items.Add(new LabrysAxe());
                    }

                    placed++;
                }
            }
        }
    }
}
