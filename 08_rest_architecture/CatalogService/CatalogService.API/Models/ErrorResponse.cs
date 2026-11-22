using System.Text.Json.Serialization;

namespace CatalogService.API.Models;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Details { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; set; }

    public ErrorResponse(int statusCode, string message, string? details = null)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }
}