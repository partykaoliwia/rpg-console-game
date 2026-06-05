namespace RpgGame.Model.Game.Behaviors;

public sealed class EnemyBehaviorContext
{
    private static readonly IReadOnlyList<(int X, int Y)> Directions =
    [
        (0, -1),
        (0, 1),
        (-1, 0),
        (1, 0)
    ];

    private readonly Board _board;
    private readonly IReadOnlyList<Player> _players;
    private readonly IReadOnlyList<Enemy> _enemies;
    private readonly Random _random;

    public EnemyBehaviorContext(
        Board board,
        IReadOnlyList<Player> players,
        IReadOnlyList<Enemy> enemies,
        Random random)
    {
        _board = board;
        _players = players;
        _enemies = enemies;
        _random = random;
    }

    public IReadOnlyList<Player> FindVisiblePlayers(Enemy enemy)
    {
        var visiblePlayers = new List<Player>();
        foreach (var direction in Directions)
        {
            for (var distance = 1; distance <= enemy.VisionRange; distance++)
            {
                var x = enemy.X + direction.X * distance;
                var y = enemy.Y + direction.Y * distance;
                var tile = _board.GetTile(x, y);
                if (!tile.IsWalkable)
                {
                    break;
                }

                var player = FindLivingPlayerAt(x, y);
                if (player != null)
                {
                    visiblePlayers.Add(player);
                    break;
                }
            }
        }

        return visiblePlayers;
    }

    public Player? FindNearestVisiblePlayer(Enemy enemy)
    {
        return FindVisiblePlayers(enemy)
            .OrderBy(player => Distance(enemy.X, enemy.Y, player.X, player.Y))
            .ThenBy(player => player.Id)
            .FirstOrDefault();
    }

    public EnemyBehaviorDecision PickRandomMove(Enemy enemy, bool includeStay)
    {
        var options = GetAvailableMoves(enemy).ToList();
        if (includeStay)
        {
            options.Add((0, 0));
        }

        if (options.Count == 0)
        {
            return EnemyBehaviorDecision.None();
        }

        var move = options[_random.Next(options.Count)];
        return move.X == 0 && move.Y == 0
            ? EnemyBehaviorDecision.None()
            : EnemyBehaviorDecision.Move(move.X, move.Y);
    }

    public EnemyBehaviorDecision PickMoveToward(Enemy enemy, int targetX, int targetY)
    {
        var currentDistance = Distance(enemy.X, enemy.Y, targetX, targetY);
        var options = GetAvailableMoves(enemy)
            .Where(move => Distance(enemy.X + move.X, enemy.Y + move.Y, targetX, targetY) < currentDistance)
            .OrderBy(move => Distance(enemy.X + move.X, enemy.Y + move.Y, targetX, targetY))
            .ToList();

        return PickMove(options);
    }

    public EnemyBehaviorDecision PickMoveAwayFromPoint(Enemy enemy, int sourceX, int sourceY)
    {
        var currentDistance = Distance(enemy.X, enemy.Y, sourceX, sourceY);
        var options = GetAvailableMoves(enemy)
            .Where(move => Distance(enemy.X + move.X, enemy.Y + move.Y, sourceX, sourceY) > currentDistance)
            .ToList();

        return PickMove(options);
    }

    public EnemyBehaviorDecision PickMoveAwayFromPlayers(Enemy enemy, IReadOnlyList<Player> visiblePlayers)
    {
        var blockedDirections = CreateDirectionsTowardPlayers(enemy, visiblePlayers);
        var options = GetAvailableMoves(enemy)
            .Where(move => !blockedDirections.Contains(move))
            .ToList();

        return PickMove(options);
    }

    public bool HasAnyAllowedDirectionAwayFromPlayers(Enemy enemy, IReadOnlyList<Player> visiblePlayers)
    {
        var blockedDirections = CreateDirectionsTowardPlayers(enemy, visiblePlayers);
        return Directions.Any(direction => !blockedDirections.Contains(direction));
    }

    public static bool AreAdjacent(Enemy enemy, Player player)
    {
        return Distance(enemy.X, enemy.Y, player.X, player.Y) == 1;
    }

    private List<(int X, int Y)> GetAvailableMoves(Enemy enemy)
    {
        return Directions
            .Where(direction => CanMove(enemy, direction.X, direction.Y))
            .ToList();
    }

    private bool CanMove(Enemy enemy, int deltaX, int deltaY)
    {
        var x = enemy.X + deltaX;
        var y = enemy.Y + deltaY;
        return _board.GetTile(x, y).IsWalkable &&
            _enemies.All(other => ReferenceEquals(other, enemy) ||
                !other.IsAlive ||
                other.X != x ||
                other.Y != y);
    }

    private Player? FindLivingPlayerAt(int x, int y)
    {
        return _players.FirstOrDefault(player => player.IsAlive && player.X == x && player.Y == y);
    }

    private EnemyBehaviorDecision PickMove(IReadOnlyList<(int X, int Y)> options)
    {
        if (options.Count == 0)
        {
            return EnemyBehaviorDecision.None();
        }

        var move = options[_random.Next(options.Count)];
        return EnemyBehaviorDecision.Move(move.X, move.Y);
    }

    private static HashSet<(int X, int Y)> CreateDirectionsTowardPlayers(
        Enemy enemy,
        IReadOnlyList<Player> visiblePlayers)
    {
        var directions = new HashSet<(int X, int Y)>();
        foreach (var player in visiblePlayers)
        {
            var deltaX = Math.Sign(player.X - enemy.X);
            var deltaY = Math.Sign(player.Y - enemy.Y);
            directions.Add((deltaX, deltaY));
        }

        return directions;
    }

    private static int Distance(int ax, int ay, int bx, int by)
    {
        return Math.Abs(ax - bx) + Math.Abs(ay - by);
    }
}
