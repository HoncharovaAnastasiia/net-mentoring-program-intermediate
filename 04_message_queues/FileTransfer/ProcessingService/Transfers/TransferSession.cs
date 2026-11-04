using FileTransfer.Domain.Messages;

namespace ProcessingService.Transfers;

public class TransferSession
{
    public FileManifest Manifest { get; }
    public string TempFilePath { get; }
    public bool[] ReceivedChunks { get; }
    public DateTimeOffset LastUpdatedUtc { get; private set; }

    public TransferSession(FileManifest manifest, string tempFilePath)
    {
        Manifest = manifest;
        TempFilePath = tempFilePath;
        ReceivedChunks = new bool[manifest.TotalChunks];
        LastUpdatedUtc = DateTimeOffset.UtcNow;
    }

    public void MarkChunkReceived(int index)
    {
        ReceivedChunks[index] = true;
        LastUpdatedUtc = DateTimeOffset.UtcNow;
    }

    public bool IsComplete() => ReceivedChunks.All(x => x);
}
