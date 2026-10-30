namespace FileTransfer.Shared.Helpers;

public static class ChunkCalculator
{
    public static int GetTotalChunks(long totalBytes, int chunkSizeBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(totalBytes);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(chunkSizeBytes);

        return (int)((totalBytes + chunkSizeBytes - 1) / chunkSizeBytes);
    }

    public static long GetChunkOffset(int chunkIndex, int chunkSizeBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(chunkIndex);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(chunkSizeBytes);

        return (long)chunkIndex * chunkSizeBytes;
    }
}
