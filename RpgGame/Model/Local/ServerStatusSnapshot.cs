namespace RpgGame.Model.Local;

public sealed class ServerStatusSnapshot
{
    public ServerStatusSnapshot(
        int port,
        DateTime startedUtc,
        int connectedClients,
        int readyClients,
        int broadcastCount,
        int stateMessageCount,
        bool isRunning)
    {
        Port = port;
        StartedUtc = startedUtc;
        ConnectedClients = connectedClients;
        ReadyClients = readyClients;
        BroadcastCount = broadcastCount;
        StateMessageCount = stateMessageCount;
        IsRunning = isRunning;
    }

    public int Port { get; }
    public DateTime StartedUtc { get; }
    public int ConnectedClients { get; }
    public int ReadyClients { get; }
    public int BroadcastCount { get; }
    public int StateMessageCount { get; }
    public bool IsRunning { get; }
}
