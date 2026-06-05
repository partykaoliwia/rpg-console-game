using System.Text.Json;

namespace RpgGame.Network;

public sealed class JsonMessageWriter
{
    private readonly JsonSerializerOptions _options = JsonProtocolOptions.Create();

    public async Task WriteAsync<T>(TextWriter writer, T message, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(message, _options);
        await writer.WriteLineAsync(json);
        await writer.FlushAsync();
    }
}
