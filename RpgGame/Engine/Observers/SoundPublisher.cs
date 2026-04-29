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
        private readonly Board _board;
        private readonly HashSet<ISoundSubscriber> _subscribers = new HashSet<ISoundSubscriber>();
        public void Subscribe(ISoundSubscriber subscriber) => _subscribers.Add(subscriber);
        public void Unsubscribe(ISoundSubscriber subscriber) => _subscribers.Remove(subscriber);
        public void NotifySubscribers(int x, int y, int range)
        {
            var reachableTiles = GetAffectedBySoundTilesBFS(x, y, range);//
            foreach (var s in _subscribers)
            {
                if (reachableTiles.Contains((s.X, s.Y))) ;
                {
                    s.UpdateOnSound(x, y, range);
                }
            }
        }
        private HashSet<(int x, int y)> GetAffectedBySoundTilesBFS(int startX, int startY, int maxRange)
        {
            var visited = new HashSet<(int x, int y)>();
            var queue = new Queue<(int x, int y, int dist)>();

            queue.Enqueue((startX, startY, 0));
            visited.Add((startX, startY));

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
                    if (!visited.Contains((nx, ny)))
                    {
                        var tile = _board.GetTile(nx, ny);
                        if (tile.IsWalkable)
                        {
                            visited.Add((nx, ny));
                            queue.Enqueue((nx, ny, current.dist + 1));

                        }
                    }
                }
            }
            return visited;
        }
    }
}