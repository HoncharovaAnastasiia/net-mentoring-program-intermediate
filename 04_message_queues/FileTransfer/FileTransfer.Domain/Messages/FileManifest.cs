namespace FileTransfer.Domain.Messages;

public record FileManifest(
    Guid FileId,
    string OriginalFileName,
    long TotalBytes,
    int ChunkSizeBytes,
    int TotalChunks,
    string FileSha256Hex,
    string? SourceInstanceId);
