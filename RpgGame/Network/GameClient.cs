using System.Net.Sockets;
using RpgGame.Dto;

namespace RpgGame.Network;

public sealed class GameClient : IDisposable
{
    private readonly JsonMessageReader _reader = new();
    private readonly JsonMessageWriter _writer = new();
    private TcpClient? _client;
    private StreamReader? _streamReader;
    private StreamWriter? _streamWriter;

    public async Task<WelcomeDto> ConnectAsync(string host, int port, string playerName, CancellationToken cancellationToken)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(host, port, cancellationToken);
        var stream = _client.GetStream();
        _streamReader = new StreamReader(stream);
        _streamWriter = new StreamWriter(stream);

        await _writer.WriteAsync(_streamWriter, new JoinGameDto { PlayerName = playerName }, cancellationToken);
        var welcome = await _reader.ReadAsync<WelcomeDto>(_streamReader, cancellationToken);
        return welcome ?? throw new InvalidOperationException("Server closed connection before sending welcome message.");
    }

    public Task SendCommandAsync(ClientCommandDto command, CancellationToken cancellationToken)
    {
        if (_streamWriter is null)
        {
            throw new InvalidOperationException("Client is not connected.");
        }

        return _writer.WriteAsync(_streamWriter, command, cancellationToken);
    }

    public Task<ServerStateDto?> ReadStateAsync(CancellationToken cancellationToken)
    {
        if (_streamReader is null)
        {
            throw new InvalidOperationException("Client is not connected.");
        }

        return _reader.ReadAsync<ServerStateDto>(_streamReader, cancellationToken);
    }

    public void Dispose()
    {
        _streamReader?.Dispose();
        _streamWriter?.Dispose();
        _client?.Dispose();
    }
}
