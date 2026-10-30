namespace FileTransfer.Domain.Messages;

public record DeadLetterRecord
{
    public DateTimeOffset OccurredUtc { get; init; } = DateTimeOffset.UtcNow;

    public required string SourceTopic { get; init; }

    public int? SourcePartition { get; init; }

    public long? SourceOffset { get; init; }

    public required string Reason { get; init; }

    public Guid? FileId { get; init; }

    public int? ChunkIndex { get; init; }

    public string? OriginalPayload { get; init; }

    public Dictionary<string, string>? Diagnostics { get; init; }
}
