using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using RpgGame.Controller.Server;
using RpgGame.Dto;

namespace RpgGame.Network;

public sealed class GameServer
{
    private const int TickDelayMs = 50;

    private readonly ServerController _controller;
    private readonly JsonMessageReader _reader = new();
    private readonly JsonMessageWriter _writer = new();
    private readonly List<ClientEndpoint> _clients = new();
    private readonly object _clientsLock = new();
    private readonly SemaphoreSlim _broadcastLock = new(1, 1);
    private int _broadcastCount;
    private int _stateMessageCount;

    public GameServer(ServerController controller)
    {
        _controller = controller;
    }

    public int ConnectedClientCount
    {
        get
        {
            lock (_clientsLock)
            {
                return _clients.Count;
            }
        }
    }

    public int ReadyClientCount
    {
        get
        {
            lock (_clientsLock)
            {
                return _clients.Count(client => client.IsReady);
            }
        }
    }

    public int BroadcastCount => Volatile.Read(ref _broadcastCount);
    public int StateMessageCount => Volatile.Read(ref _stateMessageCount);

    public async Task RunAsync(int port, CancellationToken cancellationToken)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        try
        {
            _ = RunTicksAsync(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                var tcpClient = await listener.AcceptTcpClientAsync(cancellationToken);
                if (ClientCount >= 9)
                {
                    tcpClient.Dispose();
                    continue;
                }

                _ = HandleClientAsync(tcpClient, cancellationToken);
            }
        }
        finally
        {
            listener.Stop();
        }
    }

    private int ClientCount
    {
        get
        {
            lock (_clientsLock)
            {
                return _clients.Count;
            }
        }
    }

    private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken cancellationToken)
    {
        await using var stream = tcpClient.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream);
        var endpoint = new ClientEndpoint(tcpClient, writer);

        lock (_clientsLock)
        {
            _clients.Add(endpoint);
        }

        try
        {
            var join = await _reader.ReadAsync<JoinGameDto>(reader, cancellationToken);
            if (join is null)
            {
                return;
            }

            var welcome = _controller.ConnectPlayer(join.PlayerName);
            var connectedPlayerId = welcome.PlayerId;
            endpoint.AssignPlayer(connectedPlayerId);
            await endpoint.WriteWelcomeAsync(_writer, welcome, cancellationToken);
            endpoint.MarkReady();
            await BroadcastAsync(_controller.CreateState(), cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                ClientCommandDto? command;
                try
                {
                    command = await _reader.ReadAsync<ClientCommandDto>(reader, cancellationToken);
                }
                catch (JsonException exception)
                {
                    var rejectedState = _controller.RejectMalformedCommand(
                        connectedPlayerId,
                        $"invalid JSON command: {exception.Message}");
                    await BroadcastAsync(rejectedState, cancellationToken);
                    continue;
                }

                if (command is null)
                {
                    return;
                }

                var state = _controller.Handle(connectedPlayerId, command);
                await BroadcastAsync(state, cancellationToken);
            }
        }
        finally
        {
            lock (_clientsLock)
            {
                _clients.Remove(endpoint);
            }

            tcpClient.Dispose();

            if (endpoint.PlayerId.HasValue && !cancellationToken.IsCancellationRequested)
            {
                var state = _controller.DisconnectPlayer(endpoint.PlayerId.Value);
                if (state != null)
                {
                    await BroadcastAsync(state, cancellationToken);
                }
            }
        }
    }

    private async Task RunTicksAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TickDelayMs, cancellationToken);
                var state = _controller.Tick();
                if (state != null)
                {
                    await BroadcastAsync(state, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task BroadcastAsync(ServerStateDto state, CancellationToken cancellationToken)
    {
        var disconnectedPlayers = new List<int>();
        Interlocked.Increment(ref _broadcastCount);

        await _broadcastLock.WaitAsync(cancellationToken);
        try
        {
            List<ClientEndpoint> clients;
            lock (_clientsLock)
            {
                clients = _clients.ToList();
            }

            foreach (var client in clients)
            {
                if (client.IsReady)
                {
                    if (!await TryWriteStateAsync(client, state, cancellationToken))
                    {
                        RemoveClient(client, disconnectedPlayers);
                    }
                }
            }

            foreach (var playerId in disconnectedPlayers)
            {
                _controller.DisconnectPlayer(playerId);
            }

            if (disconnectedPlayers.Count > 0)
            {
                await BroadcastCurrentStateAsync(cancellationToken);
            }
        }
        finally
        {
            _broadcastLock.Release();
        }
    }

    private async Task BroadcastCurrentStateAsync(CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _broadcastCount);

        List<ClientEndpoint> clients;
        lock (_clientsLock)
        {
            clients = _clients.ToList();
        }

        var state = _controller.CreateState();
        var disconnectedPlayers = new List<int>();
        foreach (var client in clients)
        {
            if (client.IsReady)
            {
                if (!await TryWriteStateAsync(client, state, cancellationToken))
                {
                    RemoveClient(client, disconnectedPlayers);
                }
            }
        }

        foreach (var playerId in disconnectedPlayers)
        {
            _controller.DisconnectPlayer(playerId);
        }
    }

    private async Task<bool> TryWriteStateAsync(
        ClientEndpoint client,
        ServerStateDto state,
        CancellationToken cancellationToken)
    {
        if (!client.IsConnected)
        {
            return false;
        }

        try
        {
            await client.WriteAsync(_writer, state, cancellationToken);
            Interlocked.Increment(ref _stateMessageCount);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
    }

    private void RemoveClient(ClientEndpoint client, List<int> disconnectedPlayers)
    {
        lock (_clientsLock)
        {
            _clients.Remove(client);
        }

        if (client.PlayerId.HasValue)
        {
            disconnectedPlayers.Add(client.PlayerId.Value);
        }
    }

    private sealed class ClientEndpoint
    {
        private readonly TcpClient _client;
        private readonly StreamWriter _writer;

        public ClientEndpoint(TcpClient client, StreamWriter writer)
        {
            _client = client;
            _writer = writer;
        }

        public bool IsConnected => _client.Connected;
        public bool IsReady { get; private set; }
        public int? PlayerId { get; private set; }

        public void AssignPlayer(int playerId)
        {
            PlayerId = playerId;
        }

        public void MarkReady()
        {
            IsReady = true;
        }

        public Task WriteAsync(JsonMessageWriter writer, ServerStateDto state, CancellationToken cancellationToken)
        {
            return IsConnected
                ? writer.WriteAsync(_writer, state, cancellationToken)
                : Task.CompletedTask;
        }

        public Task WriteWelcomeAsync(JsonMessageWriter writer, WelcomeDto welcome, CancellationToken cancellationToken)
        {
            return IsConnected
                ? writer.WriteAsync(_writer, welcome, cancellationToken)
                : Task.CompletedTask;
        }
    }
}
