namespace RpgGame.Model.Game;

public sealed class Board
{
    private readonly ITile[,] _tiles;

    public Board(int width = 40, int height = 20)
    {
        Width = width;
        Height = height;
        _tiles = new ITile[width, height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                _tiles[x, y] = new FloorTile();
            }
        }
    }

    public int Width { get; }
    public int Height { get; }

    public bool IsInside(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public ITile GetTile(int x, int y)
    {
        if (!IsInside(x, y))
        {
            return WallTile.OutsideBoard;
        }

        return _tiles[x, y];
    }

    public void SetTile(int x, int y, ITile tile)
    {
        if (!IsInside(x, y))
        {
            return;
        }

        _tiles[x, y] = tile;
    }

    public static Board CreateDemoBoard()
    {
        var board = new Board();

        for (var x = 0; x < board.Width; x++)
        {
            board.SetTile(x, 0, new WallTile());
            board.SetTile(x, board.Height - 1, new WallTile());
        }

        for (var y = 0; y < board.Height; y++)
        {
            board.SetTile(0, y, new WallTile());
            board.SetTile(board.Width - 1, y, new WallTile());
        }

        for (var x = 8; x < 16; x++)
        {
            board.SetTile(x, 8, new WallTile());
        }

        return board;
    }
}
