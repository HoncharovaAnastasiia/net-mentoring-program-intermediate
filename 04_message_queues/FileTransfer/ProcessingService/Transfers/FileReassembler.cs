using FileTransfer.Domain.Messages;
using FileTransfer.Shared.Generators;
using System.Security.Cryptography;

namespace ProcessingService.Transfers;

public static class FileReassembler
{
    public static void EnsureTempFile(string tempFilePath, long totalBytes)
    {
        var dir = Path.GetDirectoryName(tempFilePath);

        if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);
        if (File.Exists(tempFilePath)) return;

        using var fs = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
        fs.SetLength(totalBytes);
    }

    public static async Task WriteChunkAsync(string tempFilePath, FileChunk chunk, int chunkSizeBytes, CancellationToken ct)
    {
        var offset = (long)chunk.ChunkIndex * chunkSizeBytes;
        var bufferSize = 1024 * 1024;

        await using var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.Write, FileShare.Read, bufferSize, useAsync: true);

        fs.Seek(offset, SeekOrigin.Begin);
        await fs.WriteAsync(chunk.ChunkBytes, ct);
        await fs.FlushAsync(ct);
    }

    public static async Task<string> ComputeFileSha256HexAsync(string filePath, CancellationToken ct)
    {
        await using var stream = File.OpenRead(filePath);
        var hash = await SHA256.HashDataAsync(stream, ct);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static void MoveToOutput(string tempFilePath, string outputFolder, string originalFileName)
    {
        Directory.CreateDirectory(outputFolder);

        var safeName = MakeSafeFileName(originalFileName);
        var targetPath = UniquePathGenerator.EnsureUniqueFilePath(Path.Combine(outputFolder, safeName));

        File.Move(tempFilePath, targetPath);
    }

    private static string MakeSafeFileName(string fileName)
    {
        foreach (var character in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(character, '_');
        }

        return fileName;
    }
}
