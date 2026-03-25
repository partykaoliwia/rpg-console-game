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
            _corridorgenerator.GenerateCorridors(_board);
            
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
                    // Oddelegowanie tworzenia obiektu do naszej prostej fabryki
                    var junkItem = CreateRandomJunk();

                    tile.Items.Add(junkItem);
                    placed++;
                }
            }
        }

        // Metoda działająca jak prosta fabryka (Simple Factory)
        private Junk CreateRandomJunk()
        {
            // Losujemy liczbę od 0 do 2
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
                    return new IcarusFeather(); // Fallback w razie nieprzewidzianych błędów
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
                    // Oddelegowanie tworzenia obiektu do Fabryki
                    var weapon = CreateWeaponForSequence(placed);

                    tile.Items.Add(weapon);
                    placed++;
                }
            }
        }

        // Metoda działająca na zasadzie Fabryki (Factory)
        private Weapon CreateWeaponForSequence(int sequenceIndex)
        {
            // Przyjmuję nazwy klas broni na potrzeby przykładu:
            // Jednoręczne: ShortSword, Dagger
            // Dwuręczna: BattleAxe

            if (sequenceIndex == 0 || sequenceIndex == 2)
            {
                // Pierwsza (0) i trzecia (2) broń to losowa jednoręczna
                return _random.Next(0, 2) == 0 ? new Xiphos() : new Dory();
            }
            else if (sequenceIndex == 1)
            {
                // Druga (1) broń to GWARANTOWANA dwuręczna
                return new Labrys();
            }
            else
            {
                // Wszystkie pozostałe (3 i więcej) to pełen random z 3 dostępnych
                int roll = _random.Next(0, 3);
                if (roll == 0) return new Xiphos();
                if (roll == 1) return new Dory();
                return new Labrys();
            }
        }
    }
}
