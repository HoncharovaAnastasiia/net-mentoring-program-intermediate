using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FileTransfer.Shared.Helpers;

public static class JsonMessageSerializer
{
    private static readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static byte[] SerializeToUtf8Bytes<T>(T message) => JsonSerializer.SerializeToUtf8Bytes(message, options);

    public static T Deserialize<T>(ReadOnlySpan<byte> utf8Json)
    {
        return JsonSerializer.Deserialize<T>(utf8Json, options) ?? throw new JsonException($"Failed to deserialize {typeof(T).Name}.");
    }
}
