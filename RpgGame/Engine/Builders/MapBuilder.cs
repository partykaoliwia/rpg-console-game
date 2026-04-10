using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Gameboard;
using RpgGame.Items;
using RpgGame.Items.Modifiers;
using RpgGame.Items.Money;
using RpgGame.Items.Unusables;

namespace RpgGame.Engine.Builders
{
    public class MapBuilder : IDungeonBuilder
    {
        private Board _board;
        private Random _random = new Random();
        private DfsCorridors _corridorgenerator = new DfsCorridors();
        private readonly IFactory _factory;
        public MapBuilder(IFactory factory)
        {
            _board = new Board();
            _factory = factory;
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
            _corridorgenerator.GenerateCorridors(_board); // drazenie korytarzy DFSem
        }

        public void AddRooms()
        {
            // losujemy ilosc pokoi
            int roomCount = _random.Next(3, 7);

            for (int i = 0; i < roomCount; i++)
            {
                // losujemy wymiary
                int roomWidth = _random.Next(2, 7);
                int roomHeight = _random.Next(2, 7);

                // losujemy pozycje
                int startX = _random.Next(1, _board.Width - roomWidth - 1);
                int startY = _random.Next(1, _board.Height - roomHeight - 1);

                // drazymy pokoje
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

                if (tile.IsWalkable)
                {
                    var junkItem = _factory.CreateRandomJunk();
                    tile.Items.Add(junkItem);
                    placed++;
                }
            }
        }
        public void AddMoney(int count)
        {
            int placed = 0;
            while (placed < count)
            {
                int x = _random.Next(0, _board.Width);
                int y = _random.Next(0, _board.Height);

                var tile = _board.GetTile(x, y);
                if (tile.IsWalkable)
                {
                    var money = _factory.CreateRandomCurrency();
                    tile.Items.Add(money);
                    placed++;
                }
            }
        }
        public void AddWeapons(int count)
        {
            int placed = 0;

            while (placed < count)
            {
                int x = (placed == 0) ? 0 : _random.Next(0, _board.Width);
                int y = (placed == 0) ? 1 : _random.Next(0, _board.Height);
                var tile = _board.GetTile(x, y);

                    if (tile.IsWalkable)
                    {
                        var weapon = _factory.CreateRandomWeapon(placed);

                    tile.Items.Add(weapon);
                        placed++;
                    }
                }
        }
        public void AddEnemies(int count)
        {
            int x = _board.Width / 2;
            int y = _board.Height / 2;
            var tile = _board.GetTile(x, y);
            
            if (tile.IsWalkable)
            {
                tile.Entity = _factory.CreateBoss();
            }
            
            int placed = 1;
            while (placed < count)
            {
                x = _random.Next(0, _board.Width);
                y = _random.Next(0, _board.Height);
                tile = _board.GetTile(x, y);

                int distanceFromStart = Math.Abs(x - 1) + Math.Abs(y - 1);

                if (distanceFromStart < 10) continue;

                if (tile.IsWalkable && tile.Entity == null)
                {
                    tile.Entity = _factory.CreateEnemy(placed);
                    placed++;
                }
            }
        }
    }
}
