namespace RpgGame.Model.Local;

public sealed class ServerStatusModel
{
    private readonly object _syncRoot = new();
    private int _connectedClients;
    private int _readyClients;
    private int _broadcastCount;
    private int _stateMessageCount;
    private bool _isRunning;

    public ServerStatusModel(int port)
    {
        Port = port;
        StartedUtc = DateTime.UtcNow;
    }

    public int Port { get; }
    public DateTime StartedUtc { get; }

    public void MarkRunning()
    {
        lock (_syncRoot)
        {
            _isRunning = true;
        }
    }

    public void MarkStopped()
    {
        lock (_syncRoot)
        {
            _isRunning = false;
        }
    }

    public void Refresh(int connectedClients, int readyClients, int broadcastCount, int stateMessageCount)
    {
        lock (_syncRoot)
        {
            _connectedClients = connectedClients;
            _readyClients = readyClients;
            _broadcastCount = broadcastCount;
            _stateMessageCount = stateMessageCount;
        }
    }

    public ServerStatusSnapshot CreateSnapshot()
    {
        lock (_syncRoot)
        {
            return new ServerStatusSnapshot(
                Port,
                StartedUtc,
                _connectedClients,
                _readyClients,
                _broadcastCount,
                _stateMessageCount,
                _isRunning);
        }
    }
}
