namespace FileTransfer.Domain.Messages;

public record DeadLetterRecord(
    string SourceTopic,
    int? SourcePartition,
    long? SourceOffset,
    string Reason,
    Guid? FileId,
    int? ChunkIndex,
    string? OriginalPayload);
