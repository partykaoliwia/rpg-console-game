using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Gameboard;
using RpgGame.Items;

namespace RpgGame.Engine.Builders
{
    public class MapBuilder : IDungeonBuilder
    {
        private Board _board;
        private Random _random = new Random();
        private DfsCorridors _corridorgenerator = new DfsCorridors();
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
                int roomHeight = _random.Next(2,7);

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
                    var junkItem = CreateRandomJunk();
                    tile.Items.Add(junkItem);
                    placed++;
                }
            }
        }

        private Junk CreateRandomJunk()
        {
            int roll = _random.Next(0, 3);

            switch (roll)
            {
                case 0:
                    return new IcarusFeather();
                case 1:
                    return new CyclopsEye();
                case 2:
                    return new AriadnesThread();
                default:
                    return new IcarusFeather();
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
                    var money = CreateRandomCurrency();
                    tile.Items.Add(money);
                    placed++;
                }
            }
        }

        private Currency CreateRandomCurrency()
        {
            int[] moneyValues = { 10, 10, 10, 10, 25, 25, 25, 35, 35, 50 };
            int value = moneyValues[_random.Next(moneyValues.Length)];
            int roll = _random.Next(0, 2);

            if (roll == 0)
            {
                return new Gold(value);
            }
            else
            {
                return new Coin(value);
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
                    var weapon = CreateWeaponForSequence(placed);

                    tile.Items.Add(weapon);
                    placed++;
                }
            }
        }
        private Weapon CreateWeaponForSequence(int index)
        {
            // losowanie broni, ale tak zeby jesli jest ich wiecej niz 1 to byla gwarancja na dwureczna
            if (index == 0 || index == 2)
            {
                // Pierwsza (0) i trzecia (2) broń to losowa jednoręczna
                return _random.Next(0, 2) == 0 ? new Xiphos() : new Dory();
            }
            else if (index == 1)
            {
                // Druga (1) broń to dwuręczna
                return new Labrys();
            }
            else
            {
                // pozostale bronie losowo
                int roll = _random.Next(0, 3);
                if (roll == 0) return new Xiphos();
                if (roll == 1) return new Dory();
                return new Labrys();
            }
        }
    }
}
