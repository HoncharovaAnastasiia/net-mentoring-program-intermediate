using DataCaptureService.Configuration;
using DataCaptureService.Watching;
using FileTransfer.Shared.Generators;

namespace DataCaptureService.Runtime;

public class CaptureWorker
{
    private readonly DataCaptureOptions options;
    private readonly FolderPollingWatcher watcher;
    private readonly FileSender sender;

    public CaptureWorker(DataCaptureOptions options, FolderPollingWatcher watcher, FileSender sender)
    {
        this.options = options;
        this.watcher = watcher;
        this.sender = sender;
    }

    public async Task RunAsync(CancellationToken ct)
    {
        EnsureFolders();

        while (!ct.IsCancellationRequested)
        {
            foreach (var filePath in watcher.GetNewFiles(options.InputFolder, options.SearchPattern))
            {
                await ProcessFileSafeAsync(filePath, ct);
            }

            await Task.Delay(options.ScanInterval, ct);
        }
    }

    private void EnsureFolders()
    {
        Directory.CreateDirectory(options.InputFolder);

        if (!string.IsNullOrWhiteSpace(options.SentFolder))
        {
            Directory.CreateDirectory(options.SentFolder);
        }

        if (!string.IsNullOrWhiteSpace(options.FailedFolder))
        {
            Directory.CreateDirectory(options.FailedFolder);
        }
    }

    private async Task ProcessFileSafeAsync(string filePath, CancellationToken ct)
    {
        try
        {
            await ProcessFileAsync(filePath, ct);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send '{filePath}'. {ex.GetType().Name}: {ex.Message}");
            MoveToFailedIfConfigured(filePath);
        }
    }

    private void MoveToFailedIfConfigured(string filePath)
    {
        if (string.IsNullOrWhiteSpace(options.FailedFolder)) return;

        var targetPath = Path.Combine(options.FailedFolder, Path.GetFileName(filePath));
        targetPath = UniquePathGenerator.EnsureUniqueFilePath(targetPath);

        try
        {
            File.Move(filePath, targetPath, overwrite: false);
        }
        catch (IOException)
        {
            Console.WriteLine($"Failed to move '{filePath}' to '{targetPath}'.");
        }
    }

    private async Task ProcessFileAsync(string filePath, CancellationToken ct)
    {
        await sender.SendAsync(filePath, options.ChunkSizeBytes, options.StableFor, options.PollInterval, options.StabilityTimeout, ct);

        MoveToSentIfConfigured(filePath);
    }

    private void MoveToSentIfConfigured(string filePath)
    {
        if (!string.IsNullOrWhiteSpace(options.SentFolder))
        {
            var targetPath = Path.Combine(options.SentFolder, Path.GetFileName(filePath));
            targetPath = UniquePathGenerator.EnsureUniqueFilePath(targetPath);
            File.Move(filePath, targetPath, overwrite: false);
        }
    }
}
