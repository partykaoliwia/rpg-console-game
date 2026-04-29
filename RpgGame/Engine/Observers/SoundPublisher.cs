using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Gameboard;

namespace RpgGame.Engine.Observers
{
    public class SoundPublisher
    {
        private readonly HashSet<ISoundSubscriber> _subscribers = new HashSet<ISoundSubscriber>();
        public void Subscribe(ISoundSubscriber subscriber) => _subscribers.Add(subscriber);
        public void Unsubscribe(ISoundSubscriber subscriber) => _subscribers.Remove(subscriber);
        public void NotifySubscribers(int x, int y, int range, Board board)
        {
            var reachableTiles = GetAffectedBySoundTilesBFS(x, y, range, board);
            foreach (var s in _subscribers)
            {
                if (reachableTiles.TryGetValue((s.X,s.Y), out int distanceToEnemy))
                {
                    s.UpdateOnSound(x, y, distanceToEnemy);
                }
            }
        }
        private Dictionary<(int x,int y),int> GetAffectedBySoundTilesBFS(int startX, int startY, int maxRange, Board board)
        {
            var visited = new Dictionary<(int x, int y), int>();
            var queue = new Queue<(int x, int y, int dist)>();

            queue.Enqueue((startX, startY, 0));
            visited.Add((startX, startY),0);

            // Wektory przesunięć dla sąsiadujących pól (Góra, Dół, Prawo, Lewo)
            int[] dx = { 0, 0, 1, -1 };
            int[] dy = { 1, -1, 0, 0 };

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.dist >= maxRange) continue;
                for (int i = 0; i < 4; i++)
                {
                    int nx = current.x + dx[i];
                    int ny = current.y + dy[i];
                    if (!visited.ContainsKey((nx, ny)))
                    {
                        var tile = board.GetTile(nx, ny);
                        if (tile.IsWalkable)
                        {
                            visited.Add((nx, ny),current.dist +1);
                            queue.Enqueue((nx, ny, current.dist + 1));

                        }
                    }
                }
            }
            return visited;
        }
    }
}