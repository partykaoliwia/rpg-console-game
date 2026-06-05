using System.Text.Json;

namespace RpgGame.Network;

public sealed class JsonMessageReader
{
    private readonly JsonSerializerOptions _options = JsonProtocolOptions.Create();

    public async Task<T?> ReadAsync<T>(TextReader reader, CancellationToken cancellationToken)
    {
        var line = await reader.ReadLineAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(line))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(line, _options);
    }
}
