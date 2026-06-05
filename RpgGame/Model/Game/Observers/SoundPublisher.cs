using RpgGame.Model.Global;

namespace RpgGame.Model.Game.Observers;

public sealed class SoundPublisher
{
    private static readonly (int X, int Y)[] Directions =
    {
        (0, 1),
        (0, -1),
        (1, 0),
        (-1, 0)
    };

    private readonly HashSet<ISoundSubscriber> _subscribers = new();

    public void Subscribe(ISoundSubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public void Unsubscribe(ISoundSubscriber subscriber)
    {
        _subscribers.Remove(subscriber);
    }

    public void NotifySubscribers(int x, int y, int range, Board board, GameEventLog eventLog)
    {
        var reachableTiles = GetAffectedTiles(x, y, range, board);
        foreach (var subscriber in _subscribers.ToList())
        {
            if (reachableTiles.TryGetValue((subscriber.X, subscriber.Y), out var distance))
            {
                subscriber.UpdateOnSound(x, y, distance, eventLog);
            }
        }
    }

    private static Dictionary<(int X, int Y), int> GetAffectedTiles(int startX, int startY, int maxRange, Board board)
    {
        var visited = new Dictionary<(int X, int Y), int>();
        var queue = new Queue<(int X, int Y, int Distance)>();

        visited.Add((startX, startY), 0);
        queue.Enqueue((startX, startY, 0));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current.Distance >= maxRange)
            {
                continue;
            }

            foreach (var direction in Directions)
            {
                var nextX = current.X + direction.X;
                var nextY = current.Y + direction.Y;

                if (visited.ContainsKey((nextX, nextY)))
                {
                    continue;
                }

                var tile = board.GetTile(nextX, nextY);
                if (!tile.IsWalkable)
                {
                    continue;
                }

                visited.Add((nextX, nextY), current.Distance + 1);
                queue.Enqueue((nextX, nextY, current.Distance + 1));
            }
        }

        return visited;
    }
}
