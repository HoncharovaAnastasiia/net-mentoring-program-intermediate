namespace FileTransfer.Domain.Messages;

public record FileChunk
{
    public required Guid FileId { get; init; }

    public required int ChunkIndex { get; init; }

    public required int TotalChunks { get; init; }

    public required string ChunkSha256Hex { get; init; }

    public required byte[] ChunkBytes { get; init; }

    public DateTimeOffset CreatedUtc { get; init; } = DateTimeOffset.UtcNow;
}