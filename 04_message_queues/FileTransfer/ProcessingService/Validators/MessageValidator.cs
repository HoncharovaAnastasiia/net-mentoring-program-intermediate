using FileTransfer.Domain.Messages;
using System.Security.Cryptography;

namespace ProcessingService.Validators;

public static class MessageValidator
{
    public static void ValidateManifest(FileManifest manifest)
    {
        if (manifest.FileId == Guid.Empty) throw new InvalidOperationException("Manifest FileId is empty.");
        if (manifest.TotalBytes <= 0) throw new InvalidOperationException("Manifest TotalBytes must be > 0.");
        if (manifest.ChunkSizeBytes <= 0) throw new InvalidOperationException("Manifest ChunkSizeBytes must be > 0.");
        if (manifest.TotalChunks <= 0) throw new InvalidOperationException("Manifest TotalChunks must be > 0.");
        if (string.IsNullOrWhiteSpace(manifest.FileSha256Hex)) throw new InvalidOperationException("Manifest FileSha256Hex missing.");
        if (string.IsNullOrWhiteSpace(manifest.OriginalFileName)) throw new InvalidOperationException("Manifest OriginalFileName missing.");
    }

    public static void ValidateChunkBasics(FileChunk chunk)
    {
        if (chunk.FileId == Guid.Empty) throw new InvalidOperationException("Chunk FileId is empty.");
        if (chunk.TotalChunks <= 0) throw new InvalidOperationException("Chunk TotalChunks must be > 0.");
        if (chunk.ChunkIndex < 0) throw new InvalidOperationException("Chunk ChunkIndex must be >= 0.");
        if (chunk.ChunkBytes is null || chunk.ChunkBytes.Length == 0) throw new InvalidOperationException("Chunk ChunkBytes missing.");
        if (string.IsNullOrWhiteSpace(chunk.ChunkSha256Hex)) throw new InvalidOperationException("Chunk ChunkSha256Hex missing.");
    }

    public static void ValidateChunkIndex(FileChunk chunk, FileManifest manifest)
    {
        if (chunk.ChunkIndex < 0 || chunk.ChunkIndex >= manifest.TotalChunks)
        {
            throw new InvalidOperationException($"ChunkIndex {chunk.ChunkIndex} is out of range.");
        }
    }

    public static void EnsureChunkHashValid(FileChunk chunk)
    {
        var hash = SHA256.HashData(chunk.ChunkBytes);
        var hex = Convert.ToHexString(hash).ToLowerInvariant();

        if (!string.Equals(hex, chunk.ChunkSha256Hex, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Chunk hash mismatch.");
        }
    }
}
