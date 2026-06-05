using System.Text;
using RpgGame.Model.Game;
using RpgGame.Model.Local;
using RpgGame.View.Visitors;

namespace RpgGame.View;

public sealed class ConsoleServerView
{
    private const int ScreenWidth = 135;
    private const int ScreenHeight = 37;
    private const int MaxPlayers = 9;
    private const int Margin = 2;
    private const int Gutter = 2;
    private const int BoardBoxX = Margin;
    private const int BoardBoxY = 2;
    private const int BoardBoxWidth = 44;
    private const int BoardBoxHeight = 22;
    private const int RightColumnX = BoardBoxX + BoardBoxWidth + Gutter;
    private const int RightColumnWidth = ScreenWidth - RightColumnX - Margin;
    private const int BottomRowY = BoardBoxY + BoardBoxHeight + 1;
    private const int BottomBoxHeight = ScreenHeight - BottomRowY - 1;
    private const int PlayersBoxHeight = 15;
    private const int EventLogBoxY = BoardBoxY + PlayersBoxHeight + 1;
    private const int EventLogBoxHeight = ScreenHeight - EventLogBoxY - 1;

    public void Render(GameModel gameModel, ServerStatusModel statusModel)
    {
        Console.CursorVisible = false;
        TryPrepareConsoleSurface();

        var frame = new ServerFrame(GetFrameWidth(), GetFrameHeight());
        if (frame.Width < ScreenWidth || frame.Height < ScreenHeight)
        {
            DrawResizeMessage(frame);
            FlushFrame(frame);
            return;
        }

        var status = statusModel.CreateSnapshot();
        List<string> players;
        List<string> events;

        DrawHeader(frame);
        DrawStatusBox(frame, status);

        lock (gameModel.SyncRoot)
        {
            DrawBoardBox(frame, gameModel);
            players = gameModel.Players
                .OrderBy(player => player.Id)
                .Select(player => $"{player.Id,2}  {Trim(player.Name, 18),-18} HP:{player.Health,3}  ({player.X,2},{player.Y,2})")
                .ToList();
            events = gameModel.EventLog.GetRecent(12).ToList();
        }

        DrawPlayersBox(frame, players);
        DrawEventsBox(frame, events);
        FlushFrame(frame);
    }

    private static void DrawHeader(ServerFrame frame)
    {
        WriteCentered(frame, ScreenWidth / 2, 0, "RPG GAME SERVER", ScreenWidth - 4);
    }

    private static void DrawBoardBox(ServerFrame frame, GameModel gameModel)
    {
        DrawBox(frame, BoardBoxX, BoardBoxY, BoardBoxWidth, BoardBoxHeight, " BOARD ");
        var offsetX = BoardBoxX + 2;
        var offsetY = BoardBoxY + 1;
        var enemiesByPosition = CreateEnemyPositionIndex(gameModel);
        var playersByPosition = CreatePlayerPositionIndex(gameModel);
        var boardWidth = Math.Min(gameModel.Board.Width, BoardBoxWidth - 4);
        var boardHeight = Math.Min(gameModel.Board.Height, BoardBoxHeight - 2);

        for (var y = 0; y < boardHeight; y++)
        {
            for (var x = 0; x < boardWidth; x++)
            {
                var tile = gameModel.Board.GetTile(x, y);
                var icon = GetTileIcon(tile);

                var topItem = tile.GetTopItem();
                if (topItem != null)
                {
                    var itemVisitor = new ItemVisitor();
                    topItem.Accept(itemVisitor);
                    icon = itemVisitor.Icon;
                }

                var point = new BoardPoint(x, y);
                if (enemiesByPosition.TryGetValue(point, out var enemy))
                {
                    var characterVisitor = new CharacterVisitor(0);
                    enemy.Accept(characterVisitor);
                    icon = characterVisitor.Icon;
                }

                if (playersByPosition.TryGetValue(point, out var player))
                {
                    var characterVisitor = new CharacterVisitor(player.Id);
                    player.Accept(characterVisitor);
                    icon = characterVisitor.Icon;
                }

                frame.WriteText(offsetX + x, offsetY + y, IconSymbol(icon), 1);
            }
        }
    }

    private static void DrawPlayersBox(ServerFrame frame, IReadOnlyList<string> players)
    {
        DrawBox(frame, RightColumnX, BoardBoxY, RightColumnWidth, PlayersBoxHeight, " PLAYERS ");
        var textX = RightColumnX + 2;
        var textWidth = RightColumnWidth - 4;
        frame.WriteText(textX, BoardBoxY + 2, "ID  Name               Health  Position", textWidth);

        for(int i=0; i<9;i++)
        {
            frame.WriteText(textX, BoardBoxY + 5 + i,$" {i+1}",textWidth);
        }
        if (players.Count == 0)
        {
            frame.WriteText(textX, BoardBoxY + 4, "Waiting for clients...", textWidth);
            return;
        }

        for (var index = 0; index < players.Count && index < MaxPlayers; index++)
        {
            frame.WriteText(textX, BoardBoxY + 5 + index, players[index], textWidth);
        }
    }

    private static void DrawStatusBox(ServerFrame frame, ServerStatusSnapshot status)
    {
        DrawBox(frame, BoardBoxX, BottomRowY, BoardBoxWidth, BottomBoxHeight, " SERVER ");
        var uptime = DateTime.UtcNow - status.StartedUtc;
        var state = status.IsRunning ? "running" : "stopped";
        var textX = BoardBoxX + 2;
        var textWidth = BoardBoxWidth - 4;
        var y = BottomRowY + 1;

        frame.WriteText(textX, y++, $"Endpoint: 127.0.0.1:{status.Port}", textWidth);
        frame.WriteText(textX, y++, $"Status: {state}    Clients: {status.ReadyClients}/{status.ConnectedClients}/{MaxPlayers}", textWidth);
        frame.WriteText(textX, y++, $"Broadcasts: {status.BroadcastCount}", textWidth);
        frame.WriteText(textX, y++, $"State messages: {status.StateMessageCount}", textWidth);
        frame.WriteText(textX, y++, $"Uptime: {uptime:hh\\:mm\\:ss}", textWidth);
        frame.WriteText(textX, y, "[Q/Esc] Stop server", textWidth);
    }

    private static void DrawEventsBox(ServerFrame frame, IReadOnlyList<string> events)
    {
        DrawBox(frame, RightColumnX, EventLogBoxY, RightColumnWidth, EventLogBoxHeight, " EVENT LOG ");
        var textX = RightColumnX + 2;
        var textWidth = RightColumnWidth - 4;
        var bottomY = BottomRowY + BottomBoxHeight - 1;

        if (events.Count == 0)
        {
            frame.WriteText(textX, BottomRowY + 1, "No events yet.", textWidth);
            return;
        }

        var y = EventLogBoxY + 1;
        foreach (var entry in events)
        {
            y = WriteWrapped(frame, textX, y, textWidth, bottomY - y, entry);
            if (y >= bottomY)
            {
                return;
            }
        }
    }

    private static void DrawResizeMessage(ServerFrame frame)
    {
        WriteCentered(frame, Math.Max(0, frame.Width / 2), Math.Max(0, frame.Height / 2 - 1), "Console window is too small.", frame.Width);
        WriteCentered(frame, Math.Max(0, frame.Width / 2), Math.Max(0, frame.Height / 2), "Maximise it.", frame.Width);
    }

    private static void DrawBox(ServerFrame frame, int x, int y, int width, int height, string title)
    {
        if (width < 2 || height < 2)
        {
            return;
        }

        var horizontal = new string('─', width - 2);
        frame.WriteText(x, y, $"┌{horizontal}┐", width);
        for (var row = 1; row < height - 1; row++)
        {
            frame.WriteText(x, y + row, "│", 1);
            frame.WriteText(x + width - 1, y + row, "│", 1);
        }

        frame.WriteText(x, y + height - 1, $"└{horizontal}┘", width);
        var visibleTitle = Trim(title, width - 2);
        var titleX = x + (width - visibleTitle.Length) / 2;
        frame.WriteText(titleX, y, visibleTitle, visibleTitle.Length);
    }

    private static int WriteWrapped(ServerFrame frame, int x, int y, int width, int maxLines, string text)
    {
        var currentY = y;
        foreach (var line in Wrap(text, width).Take(maxLines))
        {
            frame.WriteText(x, currentY++, line, width);
        }

        return currentY;
    }

    private static IEnumerable<string> Wrap(string text, int width)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var line = string.Empty;

        foreach (var word in words)
        {
            if (line.Length == 0)
            {
                line = Trim(word, width);
                continue;
            }

            if (line.Length + word.Length + 1 <= width)
            {
                line = $"{line} {word}";
                continue;
            }

            yield return line;
            line = Trim(word, width);
        }

        if (line.Length > 0)
        {
            yield return line;
        }
    }

    private static void WriteCentered(ServerFrame frame, int centerX, int y, string text, int maxWidth)
    {
        var visibleText = Trim(text, maxWidth);
        var x = Math.Max(0, centerX - visibleText.Length / 2);
        frame.WriteText(x, y, visibleText, visibleText.Length);
    }

    private static void FlushFrame(ServerFrame frame)
    {
        if (frame.Width <= 0 || frame.Height <= 0)
        {
            return;
        }

        var output = new StringBuilder(frame.Width * frame.Height + frame.Height);
        for (var y = 0; y < frame.Height; y++)
        {
            for (var x = 0; x < frame.Width; x++)
            {
                output.Append(frame.Cells[y * frame.Width + x]);
            }

            if (y < frame.Height - 1)
            {
                output.AppendLine();
            }
        }

        try
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(output.ToString());
        }
        catch (ArgumentOutOfRangeException)
        {
        }
    }

    private static void TryPrepareConsoleSurface()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        try
        {
            var width = Math.Max(ScreenWidth, Math.Max(Console.BufferWidth, Console.WindowWidth));
            var height = Math.Max(ScreenHeight, Math.Max(Console.BufferHeight, Console.WindowHeight));
            if (Console.BufferWidth < width || Console.BufferHeight < height)
            {
                Console.SetBufferSize(width, height);
            }
        }
        catch (IOException)
        {
        }
        catch (PlatformNotSupportedException)
        {
        }
        catch (ArgumentOutOfRangeException)
        {
        }
    }

    private static int GetFrameWidth()
    {
        return Math.Max(0, Math.Min(ScreenWidth, Math.Min(Console.WindowWidth, Console.BufferWidth)));
    }

    private static int GetFrameHeight()
    {
        return Math.Max(0, Math.Min(ScreenHeight, Math.Min(Console.WindowHeight, Console.BufferHeight)));
    }

    private static string Trim(string text, int maxLength)
    {
        if (maxLength <= 0)
        {
            return string.Empty;
        }

        return text.Length <= maxLength ? text : text[..maxLength];
    }

    private static Dictionary<BoardPoint, Enemy> CreateEnemyPositionIndex(GameModel gameModel)
    {
        var index = new Dictionary<BoardPoint, Enemy>();
        foreach (var enemy in gameModel.Enemies)
        {
            if (enemy.IsAlive)
            {
                index[new BoardPoint(enemy.X, enemy.Y)] = enemy;
            }
        }

        return index;
    }

    private static Dictionary<BoardPoint, Player> CreatePlayerPositionIndex(GameModel gameModel)
    {
        var index = new Dictionary<BoardPoint, Player>();
        foreach (var player in gameModel.Players)
        {
            if (player.IsAlive)
            {
                index[new BoardPoint(player.X, player.Y)] = player;
            }
        }

        return index;
    }

    private static RenderIcon GetTileIcon(ITile tile)
    {
        var visitor = new TileVisitor();
        tile.Accept(visitor);
        return visitor.Icon;
    }

    private static string IconSymbol(RenderIcon icon)
    {
        return string.IsNullOrEmpty(icon.Symbol) ? " " : icon.Symbol[..1];
    }

    private readonly record struct BoardPoint(int X, int Y);

    private sealed class ServerFrame
    {
        public ServerFrame(int width, int height)
        {
            Width = Math.Max(0, width);
            Height = Math.Max(0, height);
            Cells = new char[Math.Max(0, width * height)];
            Array.Fill(Cells, ' ');
        }

        public int Width { get; }
        public int Height { get; }
        public char[] Cells { get; }

        public void WriteText(int x, int y, string text, int maxWidth)
        {
            if (y < 0 || y >= Height || x >= Width || maxWidth <= 0)
            {
                return;
            }

            var startX = Math.Max(0, x);
            var skipped = startX - x;
            var availableWidth = Math.Min(maxWidth - skipped, Width - startX);
            if (availableWidth <= 0)
            {
                return;
            }

            var visibleText = Trim(text, availableWidth);
            for (var offset = 0; offset < visibleText.Length; offset++)
            {
                Cells[y * Width + startX + offset] = visibleText[offset];
            }
        }
    }
}
