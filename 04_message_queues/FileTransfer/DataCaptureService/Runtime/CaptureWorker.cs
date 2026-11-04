using DataCaptureService.Configuration;

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
        }
        finally
        {
            watcher.MarkAsProcessed(filePath);
        }
    }

    private async Task ProcessFileAsync(string filePath, CancellationToken ct)
    {
        await sender.SendAsync(filePath, options.ChunkSizeBytes, options.StableFor, options.PollInterval, options.StabilityTimeout, ct);

        MoveToSentIfConfigured(filePath);
    }

    private void MoveToSentIfConfigured(string filePath)
    {
        if (string.IsNullOrWhiteSpace(options.SentFolder)) return;

        var targetPath = Path.Combine(options.SentFolder, Path.GetFileName(filePath));
        File.Move(filePath, targetPath, overwrite: true);
    }
}
