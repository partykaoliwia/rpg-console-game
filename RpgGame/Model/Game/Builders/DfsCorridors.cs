namespace RpgGame.Model.Game.Builders;

public sealed class DfsCorridors
{
    private readonly Random _random = new();

    public void GenerateCorridors(Board board)
    {
        var stack = new Stack<(int X, int Y)>();
        const int startX = 1;
        const int startY = 1;
        board.SetTile(startX, startY, new FloorTile());
        stack.Push((startX, startY));

        while (stack.Count > 0)
        {
            var current = stack.Peek();
            var unvisitedNeighbors = GetUnvisitedNeighbors(board, current.X, current.Y);

            if (unvisitedNeighbors.Count == 0)
            {
                stack.Pop();
                continue;
            }

            var next = unvisitedNeighbors[_random.Next(unvisitedNeighbors.Count)];
            var midX = current.X + (next.X - current.X) / 2;
            var midY = current.Y + (next.Y - current.Y) / 2;

            board.SetTile(midX, midY, new FloorTile());
            board.SetTile(next.X, next.Y, new FloorTile());
            stack.Push(next);
        }

        board.SetTile(0, 0, new FloorTile());
        board.SetTile(0, 1, new FloorTile());

        var searchY = board.Height - 2;
        if (searchY % 2 == 0)
        {
            searchY--;
        }

        for (var x = board.Width - 1; x > 0; x--)
        {
            if (!board.GetTile(x, searchY).IsWalkable)
            {
                continue;
            }

            for (var y = searchY + 1; y < board.Height; y++)
            {
                board.SetTile(x, y, new FloorTile());
            }

            break;
        }
    }

    private static List<(int X, int Y)> GetUnvisitedNeighbors(Board board, int x, int y)
    {
        var neighbors = new List<(int X, int Y)>();
        var directions = new (int X, int Y)[]
        {
            (0, -2),
            (0, 2),
            (-2, 0),
            (2, 0)
        };

        foreach (var direction in directions)
        {
            var newX = x + direction.X;
            var newY = y + direction.Y;

            if (newX > 0 &&
                newX < board.Width - 1 &&
                newY > 0 &&
                newY < board.Height - 1 &&
                !board.GetTile(newX, newY).IsWalkable)
            {
                neighbors.Add((newX, newY));
            }
        }

        return neighbors;
    }
}
