namespace FileTransfer.Domain.Messages;

public record FileChunk(
    Guid FileId,
    int ChunkIndex,
    int TotalChunks,
    string ChunkSha256Hex,
    byte[] ChunkBytes);