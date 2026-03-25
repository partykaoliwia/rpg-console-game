using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Gameboard;

namespace RpgGame.Engine.Builders
{
    // generowanie labiryntu przy użyciu DFS
    public class DfsCorridors
    {
        private Random _random = new Random();

        public void GenerateCorridors(Board board)
        {
            Stack<(int x, int y)> stack = new Stack<(int, int)>();
            int startX = 1;
            int startY = 1;
            board.SetTile(startX, startY, new EmptyTile());
            stack.Push((startX, startY));

            // drazenie korytarzy
            while(stack.Count>0)
            {
                var current = stack.Peek();
                var unvisitedNeighbors = GetUnvisitedNeighbors(board, current.x, current.y);
                if (unvisitedNeighbors.Count > 0)
                {
                    // wybieramy losowa sciane oddalona o dwa pola w dowolnym kierunku
                    var next = unvisitedNeighbors[_random.Next(unvisitedNeighbors.Count)];

                    // sciana pomiedzy
                    int midX = current.x + (next.x - current.x) / 2;
                    int midY = current.y + (next.y - current.y) / 2;

                    board.SetTile(midX, midY, new EmptyTile());
                    board.SetTile(next.x, next.y, new EmptyTile());
                    stack.Push(next);
                }
                else
                {
                    stack.Pop();
                }
            }
            // wejscie do labiryntu
            board.SetTile(0, 1, new EmptyTile());
            board.SetTile(0, 0, new EmptyTile());

            // wyjscie z labiryntu
            int searchY = board.Height - 2;
            if (searchY % 2 == 0)
            {
                searchY--;
            }
            for (int i = board.Width - 1; i > 0; i--)
            {
                if (board.GetTile(i, searchY).IsWalkable)
                {
                    for (int y = searchY + 1; y < board.Height; y++)
                    {
                        board.SetTile(i, y, new EmptyTile());
                    }
                    break;
                }
            }
        }

        private List<(int x, int y)> GetUnvisitedNeighbors(Board board, int x, int y)
        {
            // zbieramy pola oddalone o dwa od naszego pola wejsciowego i zwracamy te, ktore sa sciana
            var neighbors = new List<(int x, int y)>();
            int[][] directions = new int[][] { new int[] { 0, -2 }, new int[] { 0, 2 }, new int[] { -2, 0 }, new int[] { 2, 0 } };
            foreach(var direction in directions)
            {
                int newX = x + direction[0];
                int newY = y + direction[1];
                if(newX > 0 && newX < board.Width -1 && newY > 0 && newY < board.Height-1)
                {
                    if(!board.GetTile(newX, newY).IsWalkable)
                    {
                        neighbors.Add((newX, newY));
                    }
                }
            }
            return neighbors;
        }
    }
}
