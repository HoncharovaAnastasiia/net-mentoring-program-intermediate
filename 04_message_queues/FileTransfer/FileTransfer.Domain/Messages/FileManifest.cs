namespace FileTransfer.Domain.Messages;

public record FileManifest
{
    public required Guid FileId { get; init; }

    public required string OriginalFileName { get; init; }

    public required long TotalBytes { get; init; }

    public required int ChunkSizeBytes { get; init; }

    public required int TotalChunks { get; init; }

    public required string FileSha256Hex { get; init; }

    public DateTimeOffset CreatedUtc { get; init; } = DateTimeOffset.UtcNow;

    public string? SourceInstanceId { get; init; }

    public string? MimeType { get; init; }

    public Dictionary<string, string>? Metadata { get; init; }
}
