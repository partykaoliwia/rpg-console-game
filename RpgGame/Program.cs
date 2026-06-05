using System.Net.Sockets;
using System.Text.Json;
using RpgGame.Configuration;
using RpgGame.Controller.Local;
using RpgGame.Controller.Server;
using RpgGame.Controller.Shared.Commands;
using RpgGame.Controller.SinglePlayer;
using RpgGame.Dto;
using RpgGame.Logging;
using RpgGame.Model.Game;
using RpgGame.Model.Game.Builders;
using RpgGame.Model.Game.Factories;
using RpgGame.Model.Local;
using RpgGame.Model.Local.Instructions;
using RpgGame.Model.Local.KeyBindings;
using RpgGame.Network;
using RpgGame.View;

namespace RpgGame;

internal static class Program
{
    private enum StartupMode
    {
        SinglePlayer,
        Server,
        Client
    }

    private readonly record struct DungeonThemeChoice(
        ConsoleKey PrimaryKey,
        ConsoleKey AlternateKey,
        string Label,
        string Description,
        Func<IDungeonThemeFactory> CreateFactory);

    private static readonly DungeonThemeChoice[] DungeonThemeChoices =
    {
        new(
            ConsoleKey.D1,
            ConsoleKey.NumPad1,
            "Daedalus' labyrinth",
            "Classic exploration through winding corridors and hidden paths.",
            () => new LabyrinthFactory()),
        new(
            ConsoleKey.D2,
            ConsoleKey.NumPad2,
            "Ares' arena",
            "A brutal combat-heavy dungeon with open space and dangerous champions.",
            () => new ArenaFactory()),
        new(
            ConsoleKey.D3,
            ConsoleKey.NumPad3,
            "Library of Alexandria",
            "A more arcane expedition among books, relics and ancient guardians.",
            () => new LibraryFactory()),
        new(
            ConsoleKey.R,
            ConsoleKey.R,
            "Random",
            "Let fate choose one of the available dungeon themes.",
            () => CreateRandomDungeonThemeFactory())
    };

    private static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var config = ConfigLoader.LoadOrDefault("config.json");
        var logFilePath = ConfigureLogging(config);

        if (args.Length > 0 && IsCommand(args[0], "--server", "-server"))
        {
            var port = args.Length > 1 && int.TryParse(args[1], out var parsedPort)
                ? parsedPort
                : config.DefaultPort;
            await RunServerAsync(port, config, CreateDungeonThemeFactory(config));
            return;
        }

        if (args.Length > 0 && IsCommand(args[0], "--client", "-client"))
        {
            var endpoint = args.Length > 1 ? args[1] : $"{config.DefaultHost}:{config.DefaultPort}";
            var playerName = args.Length > 2 ? args[2] : config.ClientPlayerName;
            await RunClientAsync(endpoint, config, logFilePath, playerName);
            return;
        }

        if (args.Length > 0 && IsCommand(args[0], "--single", "-single", "--singleplayer", "-singleplayer"))
        {
            RunSinglePlayer(config, logFilePath, CreateDungeonThemeFactory(config));
            return;
        }

        var startupMode = ReadStartupMode();
        if (startupMode == StartupMode.Server)
        {
            var factory = ReadDungeonThemeFactory(config);
            await RunServerAsync(ReadServerPort(config), config, factory);
            return;
        }

        if (startupMode == StartupMode.Client)
        {
            await RunClientAsync(
                ReadClientEndpoint(config),
                config,
                logFilePath,
                ReadClientPlayerName(config));
            return;
        }

        RunSinglePlayer(config, logFilePath, ReadDungeonThemeFactory(config));
    }

    private static StartupMode ReadStartupMode()
    {
        Console.Clear();
        Console.WriteLine("RPG Game");
        Console.WriteLine();
        Console.WriteLine("[1] Singleplayer");
        Console.WriteLine("[S] Server");
        Console.WriteLine("[K] Client");
        Console.WriteLine();
        Console.Write("Choose mode [1]: ");

        var key = Console.ReadKey(intercept: true).Key;
        Console.WriteLine();

        if (key == ConsoleKey.S)
        {
            return StartupMode.Server;
        }

        if (key == ConsoleKey.K || key == ConsoleKey.C)
        {
            return StartupMode.Client;
        }

        return StartupMode.SinglePlayer;
    }

    private static int ReadServerPort(GameConfig config)
    {
        Console.Write($"Server port [{config.DefaultPort}]: ");
        var input = Console.ReadLine();
        return int.TryParse(input, out var port) ? port : config.DefaultPort;
    }

    private static string ReadClientEndpoint(GameConfig config)
    {
        var defaultEndpoint = $"{config.DefaultHost}:{config.DefaultPort}";
        Console.Write($"Server endpoint [{defaultEndpoint}]: ");
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? defaultEndpoint : input.Trim();
    }

    private static string ReadClientPlayerName(GameConfig config)
    {
        Console.Write($"Player name [{config.ClientPlayerName}]: ");
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? config.ClientPlayerName : input.Trim();
    }

    private static IDungeonThemeFactory ReadDungeonThemeFactory(GameConfig config)
    {
        Console.Clear();
        Console.WriteLine("Choose dungeon theme");
        Console.WriteLine();
        WriteDungeonThemeChoice("1", DungeonThemeChoices[0]);
        WriteDungeonThemeChoice("2", DungeonThemeChoices[1]);
        WriteDungeonThemeChoice("3", DungeonThemeChoices[2]);
        WriteDungeonThemeChoice("R", DungeonThemeChoices[3]);
        Console.WriteLine();
        Console.Write($"Choose theme [{config.DungeonTheme}]: ");

        var key = Console.ReadKey(intercept: true).Key;
        Console.WriteLine();

        foreach (var choice in DungeonThemeChoices)
        {
            if (key == choice.PrimaryKey || key == choice.AlternateKey)
            {
                return choice.CreateFactory();
            }
        }

        return CreateDungeonThemeFactory(config);
    }

    private static void WriteDungeonThemeChoice(string keyText, DungeonThemeChoice choice)
    {
        Console.WriteLine($"[{keyText}] {choice.Label}");
        Console.WriteLine($"    {choice.Description}");
    }

    private static void RunSinglePlayer(GameConfig config, string logFilePath, IDungeonThemeFactory factory)
    {
        Logger.Instance.Log($"Starting singleplayer mode with theme {factory.ThemeName}.");
        var gameModel = GameModel.CreateGame(factory);
        gameModel.AddPlayer(config.PlayerName);
        var eventLogger = new GameEventLogForwarder();
        eventLogger.Flush(gameModel);
        var localModel = CreateLocalModel(1, config, factory, logFilePath);
        var view = new ConsoleGameView();
        var controller = new SinglePlayerController(
            gameModel,
            localModel,
            () => CreateKeyBindings(config),
            keyBindings => CreateInstructionGuide(factory, keyBindings));

        Console.Clear();
        while (!controller.ShouldQuit)
        {
            if (Console.KeyAvailable)
            {
                controller.HandleKey(ReadInputKey());
            }

            controller.Tick();
            eventLogger.Flush(gameModel);
            view.Render(gameModel, localModel);

            Thread.Sleep(30);
        }

        Console.ResetColor();
        Console.CursorVisible = true;
        Console.Clear();
        Logger.Instance.Log("Singleplayer mode stopped.");
    }

    private static async Task RunServerAsync(int port, GameConfig config, IDungeonThemeFactory factory)
    {
        Logger.Instance.Log($"Starting server on port {port} with theme {factory.ThemeName}.");
        var gameModel = GameModel.CreateGame(factory);
        var eventLogger = new GameEventLogForwarder();
        eventLogger.Flush(gameModel);
        var mapper = new GameStateMapper();
        var commandRegistry = new CommandRegistry();
        var controller = new ServerController(gameModel, commandRegistry, mapper);
        var serverStatus = new ServerStatusModel(port);
        var server = new GameServer(controller);
        var view = new ConsoleServerView();
        using var serverCancellation = new CancellationTokenSource();

        Console.Clear();
        var serverTask = server.RunAsync(port, serverCancellation.Token);
        serverStatus.MarkRunning();

        try
        {
            while (!serverTask.IsCompleted)
            {
                if (TryReadInputKey(out var key) && IsStopServerKey(key))
                {
                    await serverCancellation.CancelAsync();
                    break;
                }

                RefreshServerStatus(serverStatus, server);
                eventLogger.Flush(gameModel);
                view.Render(gameModel, serverStatus);
                await Task.Delay(250);
            }

            await serverTask;
        }
        catch (OperationCanceledException)
        {
        }
        catch (SocketException exception)
        {
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
            Console.WriteLine($"Could not start server on port {port}. {exception.Message}");
            Logger.Instance.Log($"Server failed to start: {exception.Message}");
            return;
        }
        finally
        {
            serverStatus.MarkStopped();
            RefreshServerStatus(serverStatus, server);
            eventLogger.Flush(gameModel);
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
        }

        Logger.Instance.Log("Server stopped.");
    }

    private static async Task RunClientAsync(
        string endpoint,
        GameConfig config,
        string logFilePath,
        string playerName)
    {
        Logger.Instance.Log($"Starting client for endpoint {endpoint}.");
        var (host, port) = ParseEndpoint(endpoint, config);

        using var client = new GameClient();
        WelcomeDto welcome;
        try
        {
            welcome = await client.ConnectAsync(host, port, playerName, CancellationToken.None);
        }
        catch (SocketException exception)
        {
            Console.WriteLine($"Could not connect to {host}:{port}. {exception.Message}");
            Logger.Instance.Log($"Client connection failed: {exception.Message}");
            return;
        }
        catch (IOException exception)
        {
            Console.WriteLine($"Connection failed while joining {host}:{port}. {exception.Message}");
            Logger.Instance.Log($"Client join failed: {exception.Message}");
            return;
        }

        var gameModel = new GameModel(new Board());
        var factory = CreateDungeonThemeFactory(welcome.State.DungeonThemeName, config);
        var localModel = CreateLocalModel(welcome.PlayerId, config, factory, logFilePath);
        var mapper = new GameStateMapper();
        mapper.ApplyTo(gameModel, welcome.State);
        var eventLogger = new GameEventLogForwarder();
        eventLogger.Flush(gameModel);
        var updateController = new ClientUpdateController(gameModel, localModel, mapper, welcome.StateSequence);
        var inputController = new ClientInputController(
            gameModel,
            localModel,
            client,
            () => CreateKeyBindings(config),
            keyBindings => CreateInstructionGuide(factory, keyBindings));
        var view = new ConsoleGameView();

        Console.Clear();
        using var readCancellation = new CancellationTokenSource();
        var serverDisconnected = 0;

        var readTask = Task.Run(async () =>
        {
            try
            {
                while (!inputController.ShouldQuit && Volatile.Read(ref serverDisconnected) == 0)
                {
                    var state = await client.ReadStateAsync(readCancellation.Token);
                    if (state == null)
                    {
                        Interlocked.Exchange(ref serverDisconnected, 1);
                        return;
                    }

                    updateController.Apply(state);
                    eventLogger.Flush(gameModel);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (IOException exception)
            {
                Logger.Instance.Log($"Server connection closed: {exception.Message}");
                Interlocked.Exchange(ref serverDisconnected, 1);
            }
            catch (ObjectDisposedException)
            {
                Interlocked.Exchange(ref serverDisconnected, 1);
            }
            catch (JsonException exception)
            {
                Logger.Instance.Log($"Server sent invalid JSON: {exception.Message}");
                Interlocked.Exchange(ref serverDisconnected, 1);
            }
        });

        while (!inputController.ShouldQuit && Volatile.Read(ref serverDisconnected) == 0)
        {
            if (Console.KeyAvailable)
            {
                await inputController.HandleKeyAsync(ReadInputKey(), CancellationToken.None);
            }

            view.Render(gameModel, localModel);

            await Task.Delay(30);
        }

        await readCancellation.CancelAsync();
        await readTask;
        var disconnected = Volatile.Read(ref serverDisconnected) == 1;

        Console.ResetColor();
        Console.CursorVisible = true;
        Console.Clear();
        if (disconnected)
        {
            Console.WriteLine("Disconnected from server.");
        }

        Logger.Instance.Log("Client stopped.");
    }

    private static void RefreshServerStatus(ServerStatusModel serverStatus, GameServer server)
    {
        serverStatus.Refresh(
            server.ConnectedClientCount,
            server.ReadyClientCount,
            server.BroadcastCount,
            server.StateMessageCount);
    }

    private static bool IsStopServerKey(ConsoleKey key)
    {
        return key == ConsoleKey.Q || key == ConsoleKey.Escape;
    }

    private static LocalModel CreateLocalModel(
        int playerId,
        GameConfig config,
        IDungeonThemeFactory factory,
        string logFilePath)
    {
        var keyBindings = CreateKeyBindings(config);
        var localModel = new LocalModel(playerId, keyBindings);
        localModel.ReplaceLogFilePath(logFilePath);
        localModel.OpenInstructions(CreateInstructionGuide(factory, keyBindings));
        return localModel;
    }

    private static (string Host, int Port) ParseEndpoint(string endpoint, GameConfig config)
    {
        var defaultEndpoint = $"{config.DefaultHost}:{config.DefaultPort}";
        var endpointText = string.IsNullOrWhiteSpace(endpoint) ? defaultEndpoint : endpoint.Trim();
        var parts = endpointText.Split(':', 2, StringSplitOptions.TrimEntries);
        var host = parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[0])
            ? parts[0]
            : config.DefaultHost;
        var port = parts.Length > 1 && int.TryParse(parts[1], out var parsedPort)
            ? parsedPort
            : config.DefaultPort;

        return (host, port);
    }

    private static KeyBindingProfile CreateKeyBindings(GameConfig config)
    {
        return new KeyBindingProfileLoader().LoadOrDefault(config.KeybindsPath);
    }

    private static InstructionGuide CreateInstructionGuide(IDungeonThemeFactory factory, KeyBindingProfile keyBindings)
    {
        var director = new DungeonDirector();
        var builder = new InstructionBuilder(factory, keyBindings);
        director.SetBuilder(builder);
        director.SetStrategy(factory.GenerationStrategy);
        director.ConstructDungeon();
        return builder.BuildResult();
    }

    private static IDungeonThemeFactory CreateDungeonThemeFactory(GameConfig config)
    {
        return CreateDungeonThemeFactory(config.DungeonTheme, config);
    }

    private static IDungeonThemeFactory CreateDungeonThemeFactory(string dungeonTheme, GameConfig config)
    {
        var factories = new Dictionary<string, Func<IDungeonThemeFactory>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Random", () => CreateRandomDungeonThemeFactory() },
            { "Labyrinth", () => new LabyrinthFactory() },
            { "Daedalus", () => new LabyrinthFactory() },
            { "Daedalus' labyrinth", () => new LabyrinthFactory() },
            { "Arena", () => new ArenaFactory() },
            { "Ares", () => new ArenaFactory() },
            { "Ares' arena", () => new ArenaFactory() },
            { "Library", () => new LibraryFactory() },
            { "Alexandria", () => new LibraryFactory() },
            { "Library of Alexandria", () => new LibraryFactory() }
        };

        var theme = string.IsNullOrWhiteSpace(dungeonTheme) ? config.DungeonTheme : dungeonTheme;
        return factories.TryGetValue(theme, out var factory)
            ? factory()
            : new LabyrinthFactory();
    }

    private static IDungeonThemeFactory CreateRandomDungeonThemeFactory()
    {
        Func<IDungeonThemeFactory>[] randomFactories =
        {
            () => new LibraryFactory(),
            () => new ArenaFactory(),
            () => new LabyrinthFactory()
        };

        return randomFactories[Random.Shared.Next(randomFactories.Length)]();
    }

    private static string ConfigureLogging(GameConfig config)
    {
        var logFilePath = CreateLogFilePath(config);
        var memoryWriter = new MemoryLogWriter();
        Logger.Instance.SetMemoryWriter(memoryWriter);
        Logger.Instance.AddWriter(new FileLogWriter(logFilePath));
        Logger.Instance.Log("Logger configured.");
        return logFilePath;
    }

    private static string CreateLogFilePath(GameConfig config)
    {
        var fileName = $"{config.PlayerName}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        return Path.Combine(config.LogDirectory, fileName);
    }

    private static ConsoleKey ReadInputKey()
    {
        var key = Console.ReadKey(true).Key;
        while (Console.KeyAvailable)
        {
            Console.ReadKey(true);
        }

        return key;
    }

    private static bool TryReadInputKey(out ConsoleKey key)
    {
        key = default;
        try
        {
            if (!Console.KeyAvailable)
            {
                return false;
            }

            key = ReadInputKey();
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private static bool IsCommand(string actual, params string[] expectedCommands)
    {
        return expectedCommands.Any(expected => string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase));
    }
}
