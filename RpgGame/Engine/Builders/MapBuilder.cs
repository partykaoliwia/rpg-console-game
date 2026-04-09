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
                int x = (placed == 0) ? 0 : _random.Next(0, _board.Width);
                int y = (placed == 0) ? 1 : _random.Next(0, _board.Height);
                var tile = _board.GetTile(x, y);

                    if (tile.IsWalkable)
                    {
                        var weapon = CreateWeaponForSequence(placed);

                        tile.Items.Add(weapon);
                        placed++;
                    }
                }
        }
        private IItem CreateWeaponForSequence(int index)
        {
            int damage = 10 + _random.Next(0, 4);
            IItem weapon;
            // losowanie broni, ale tak zeby jesli jest ich wiecej niz 1 to byla gwarancja na dwureczna
            if (index == 0 || index == 2)
            {
                // Pierwsza (0) i trzecia (2) broń to losowa jednoręczna
                weapon = _random.Next(0, 2) == 0 ? new Xiphos(damage) : new Dory(damage);
            }
            else if (index == 1)
            {
                // Druga (1) broń to dwuręczna
                weapon = new Labrys(damage);
            }
            else
            {
                // pozostale bronie losowo
                int roll = _random.Next(0, 3);
                if (roll == 0) weapon = new Xiphos(damage);
                else if (roll == 1) weapon = new Dory(damage);
                else weapon = new Labrys(damage);
            }
            int modRoll = _random.Next(0, 4);

            if (modRoll == 0) return new SpartanModifier(weapon);
            if (modRoll == 1) return new MidasTouchModifier(weapon);
            if(modRoll==2) return new IcarianModifier(new DionysianModifier(weapon));
            return new AthenianModifier(weapon); 
        }
        //public void AddEnemies(int count)
        //{
        //    Enemy enemy;
        //    int x = _board.Width / 2;
        //    int y = _board.Height / 2;
        //    var tile = _board.GetTile(x, y);
        //    enemy = new Enemy(80, 15, 5); // minotaur
        //    enemy.ItemColor = Color.SaddleBrown;
        //    if (tile.IsWalkable)
        //    {
        //        tile.Entity = enemy;
        //    }
        //    int placed = 1;
        //    while (placed < count)
        //    {
        //        x = _random.Next(0, _board.Width);
        //        y = _random.Next(0, _board.Height);
        //        if((x==0 && y == 0)||x==0 && y == 1)
        //        {
        //            continue;
        //        }
        //        tile = _board.GetTile(x, y);
        //        if (tile.IsWalkable && tile.Entity == null)
        //        {
        //            // pozostali to losowi przeciwnicy
        //            int roll = _random.Next(0, 3);
        //            if (roll == 0)
        //            {
        //                enemy = new Enemy(30, 10, 2);
        //            }
        //            else if (roll == 1)
        //            {
        //                enemy = new Enemy(40, 13, 1);
        //            }
        //            else
        //            {
        //                enemy = new Enemy(35, 15, 10);
        //            }

        //            tile.Entity = enemy;
        //            placed++;
        //        }
        //    }
        //}
        public void AddEnemies(int count)
        {
            Enemy enemy;
            int x = _board.Width / 2;
            int y = _board.Height / 2;
            var tile = _board.GetTile(x, y);
            
            enemy = new Enemy(80, 15, 5, "Minotaur"); 
            enemy.ItemColor = Color.SaddleBrown;
            if (tile.IsWalkable)
            {
                tile.Entity = enemy;
            }
            
            int placed = 1;
            while (placed < count)
            {
                x = _random.Next(0, _board.Width);
                y = _random.Next(0, _board.Height);
                tile = _board.GetTile(x, y);

                int distanceFromStart = Math.Abs(x - 1) + Math.Abs(y - 1);

                if (distanceFromStart < 10)
                {
                    continue;
                }

                if (tile.IsWalkable && tile.Entity == null)
                {
                    // Moc wrogów zależnie od odległości
                    if (distanceFromStart < 25)
                    {
                        // Blisko - słabi wrogowie
                        enemy = new Enemy(30, 20, 2);
                        //enemy.ItemColor = Color.LightGray;
                    }
                    else if (distanceFromStart < 40)
                    {
                        // Środek mapy - średni wrogowie
                        enemy = new Enemy( 40, 13, 4);
                        //enemy.ItemColor = Color.DarkOrange;
                    }
                    else
                    {
                        // Krawędzie i koniec mapy - trudni wrogowie
                        enemy = new Enemy(50, 18, 8);
                       // enemy.ItemColor = Color.Gray;
                    }

                    tile.Entity = enemy;
                    placed++;
                }
            }
        }
    }
}
