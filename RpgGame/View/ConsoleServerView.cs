using System.Text;
using RpgGame.Model.Game;
using RpgGame.Model.Local;

namespace RpgGame.View;

public sealed class ConsoleServerView
{
    private const int ScreenWidth = 120;
    private const int ScreenHeight = 29;
    private const int MaxPlayers = 9;

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
        lock (gameModel.SyncRoot)
        {
            players = gameModel.Players
                .OrderBy(player => player.Id)
                .Select(player => $"{player.Id,2}  {Trim(player.Name, 18),-18} HP:{player.Health,3}  ({player.X,2},{player.Y,2})")
                .ToList();
            events = gameModel.EventLog.GetRecent(12).ToList();
        }

        DrawHeader(frame);
        DrawStatusBox(frame, status);
        DrawPlayersBox(frame, players);
        DrawEventsBox(frame, events);
        FlushFrame(frame);
    }

    private static void DrawHeader(ServerFrame frame)
    {
        WriteCentered(frame, ScreenWidth / 2, 0, "RPG GAME SERVER", ScreenWidth - 4);
    }

    private static void DrawStatusBox(ServerFrame frame, ServerStatusSnapshot status)
    {
        DrawBox(frame, 2, 2, 33, 11, " SERVER ");
        var uptime = DateTime.UtcNow - status.StartedUtc;
        var state = status.IsRunning ? "running" : "stopped";

        frame.WriteText(4, 4, $"Port: {status.Port}", 29);
        frame.WriteText(4, 5, $"Endpoint: 127.0.0.1:{status.Port}", 29);
        frame.WriteText(4, 6, $"Status: {state}", 29);
        frame.WriteText(4, 7, $"Clients: {status.ReadyClients}/{status.ConnectedClients} / {MaxPlayers}", 29);
        frame.WriteText(4, 8, $"Broadcasts: {status.BroadcastCount}", 29);
        frame.WriteText(4, 9, $"State messages: {status.StateMessageCount}", 29);
        frame.WriteText(4, 10, $"Uptime: {uptime:hh\\:mm\\:ss}", 29);
        frame.WriteText(4, 11, "[Q/Esc] Stop server", 29);
    }

    private static void DrawPlayersBox(ServerFrame frame, IReadOnlyList<string> players)
    {
        DrawBox(frame, 37, 2, 81, 11, " PLAYERS ");
        frame.WriteText(39, 4, "ID  Name               Health  Position", 77);

        if (players.Count == 0)
        {
            frame.WriteText(39, 6, "Waiting for clients...", 77);
            return;
        }

        for (var index = 0; index < players.Count && index < MaxPlayers; index++)
        {
            frame.WriteText(39, 5 + index, players[index], 77);
        }
    }

    private static void DrawEventsBox(ServerFrame frame, IReadOnlyList<string> events)
    {
        DrawBox(frame, 2, 14, 116, 13, " EVENT LOG ");
        if (events.Count == 0)
        {
            frame.WriteText(4, 16, "No events yet.", 112);
            return;
        }

        var y = 16;
        foreach (var entry in events)
        {
            y = WriteWrapped(frame, 4, y, 112, 26 - y, entry);
            if (y >= 26)
            {
                return;
            }
        }
    }

    private static void DrawResizeMessage(ServerFrame frame)
    {
        WriteCentered(frame, Math.Max(0, frame.Width / 2), Math.Max(0, frame.Height / 2 - 1), "Console window is too small.", frame.Width);
        WriteCentered(frame, Math.Max(0, frame.Width / 2), Math.Max(0, frame.Height / 2), "Use at least 120 x 29.", frame.Width);
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
