using RpgGame.Model.Game.Factories;

namespace RpgGame.Model.Game.Builders;

public sealed class MapBuilder : IDungeonBuilder
{
    private readonly IDungeonThemeFactory _factory;
    private readonly Random _random = new();
    private readonly DfsCorridors _corridorGenerator = new();
    private readonly List<Enemy> _enemies = new();
    private Board _board = new();
    private int _nextEnemyId = 1;

    public MapBuilder(IDungeonThemeFactory factory)
    {
        _factory = factory;
    }

    public GeneratedDungeon BuildResult()
    {
        return new GeneratedDungeon(_board, _enemies.ToList());
    }

    public void BuildEmptyDungeon()
    {
        _board = new Board();
        for (var y = 0; y < _board.Height; y++)
        {
            for (var x = 0; x < _board.Width; x++)
            {
                _board.SetTile(x, y, new FloorTile());
            }
        }
    }

    public void BuildFilledDungeon()
    {
        _board = new Board();
        for (var y = 0; y < _board.Height; y++)
        {
            for (var x = 0; x < _board.Width; x++)
            {
                _board.SetTile(x, y, new WallTile());
            }
        }
    }

    public void AddCorridors()
    {
        _corridorGenerator.GenerateCorridors(_board);
    }

    public void AddRooms()
    {
        var roomCount = _random.Next(3, 7);

        for (var i = 0; i < roomCount; i++)
        {
            var roomWidth = _random.Next(2, 7);
            var roomHeight = _random.Next(2, 7);
            var startX = _random.Next(1, _board.Width - roomWidth - 1);
            var startY = _random.Next(1, _board.Height - roomHeight - 1);

            for (var y = startY; y < startY + roomHeight; y++)
            {
                for (var x = startX; x < startX + roomWidth; x++)
                {
                    _board.SetTile(x, y, new FloorTile());
                }
            }
        }
    }

    public void AddCentralHall(int width, int height)
    {
        width = Math.Min(width, _board.Width - 2);
        height = Math.Min(height, _board.Height - 2);

        var startX = (_board.Width - width) / 2;
        var startY = (_board.Height - height) / 2;

        for (var y = startY; y < startY + height; y++)
        {
            for (var x = startX; x < startX + width; x++)
            {
                _board.SetTile(x, y, new FloorTile());
            }
        }
    }

    public void AddItems(int count)
    {
        PlaceOnRandomWalkableTiles(count, (_, tile) => tile.AddItem(_factory.CreateRandomJunk()));
    }

    public void AddWeapons(int count)
    {
        var placed = 0;
        if (count <= 0)
        {
            return;
        }

        if (TryPlaceWeaponAt(0, 1, placed))
        {
            placed++;
        }

        while (placed < count)
        {
            var weaponIndex = placed;
            PlaceOnRandomWalkableTiles(1, (_, tile) => tile.AddItem(_factory.CreateRandomWeapon(weaponIndex)));
            placed++;
        }
    }

    public void AddMoney(int count)
    {
        PlaceOnRandomWalkableTiles(count, (_, tile) => tile.AddItem(_factory.CreateRandomCurrency()));
    }

    public void AddEnemies(int count)
    {
        var centerX = _board.Width / 2;
        var centerY = _board.Height / 2;

        if (_board.GetTile(centerX, centerY).IsWalkable)
        {
            _enemies.Add(_factory.CreateBoss(_nextEnemyId++, centerX, centerY));
        }

        var placed = 1;
        while (placed < count)
        {
            var x = _random.Next(0, _board.Width);
            var y = _random.Next(0, _board.Height);
            var distanceFromStart = Math.Abs(x - 1) + Math.Abs(y - 1);

            if (distanceFromStart < 10 ||
                !_board.GetTile(x, y).IsWalkable ||
                _enemies.Any(enemy => enemy.X == x && enemy.Y == y))
            {
                continue;
            }

            _enemies.Add(_factory.CreateEnemy(_nextEnemyId++, x, y, distanceFromStart));
            placed++;
        }
    }

    private void PlaceOnRandomWalkableTiles(int count, Action<(int X, int Y), ITile> place)
    {
        var placed = 0;
        while (placed < count)
        {
            var x = _random.Next(0, _board.Width);
            var y = _random.Next(0, _board.Height);
            var tile = _board.GetTile(x, y);

            if (!tile.IsWalkable)
            {
                continue;
            }

            place((x, y), tile);
            placed++;
        }
    }

    private bool TryPlaceWeaponAt(int x, int y, int index)
    {
        var tile = _board.GetTile(x, y);
        if (!tile.IsWalkable)
        {
            return false;
        }

        tile.AddItem(_factory.CreateRandomWeapon(index));
        return true;
    }
}
