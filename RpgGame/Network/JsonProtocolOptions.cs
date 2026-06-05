using System.Text.Json;
using System.Text.Json.Serialization;
using RpgGame.Dto;

namespace RpgGame.Network;

public static class JsonProtocolOptions
{
    public static JsonSerializerOptions Create()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter<AttackKind>());
        options.Converters.Add(new JsonStringEnumConverter<ClientCommandKind>());
        options.Converters.Add(new JsonStringEnumConverter<EnemyKind>());
        options.Converters.Add(new JsonStringEnumConverter<HandSlotKind>());
        options.Converters.Add(new JsonStringEnumConverter<ItemKind>());
        options.Converters.Add(new JsonStringEnumConverter<ItemModifierKind>());
        options.Converters.Add(new JsonStringEnumConverter<TileKind>());
        return options;
    }
}
