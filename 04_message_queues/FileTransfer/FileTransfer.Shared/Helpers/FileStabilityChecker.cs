namespace FileTransfer.Shared.Helpers;

public static class FileStabilityChecker
{
    public static async Task WaitUntilStableAsync(string filePath, TimeSpan stableFor, TimeSpan pollInterval, TimeSpan timeout, CancellationToken ct = default)
    {
        var started = DateTimeOffset.UtcNow;

        long? lastObservedSize = null;
        DateTimeOffset? stableSince = null;

        while (true)
        {
            ct.ThrowIfCancellationRequested();
            if (!File.Exists(filePath)) throw new FileNotFoundException("File not found.", filePath);

            var observation = TryObserve(filePath);

            UpdateStability(observation, ref lastObservedSize, ref stableSince);

            if (IsStable(stableSince, stableFor)) return;

            ThrowIfTimedOut(started, timeout, filePath);

            await Task.Delay(pollInterval, ct);
        }
    }

    private static FileObservation TryObserve(string filePath)
    {
        try
        {
            var info = new FileInfo(filePath);
            long size = info.Length;

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return FileObservation.Success(size);
        }
        catch (IOException)
        {
            return FileObservation.Unavailable();
        }
        catch (UnauthorizedAccessException)
        {
            return FileObservation.Unavailable();
        }
    }

    private static void UpdateStability(FileObservation observation, ref long? lastObservedSize, ref DateTimeOffset? stableSince)
    {
        if (!observation.IsAvailable)
        {
            stableSince = null;
            return;
        }

        if (lastObservedSize.HasValue && observation.SizeBytes == lastObservedSize.Value)
        {
            stableSince ??= DateTimeOffset.UtcNow;
        }
        else
        {
            stableSince = null;
            lastObservedSize = observation.SizeBytes;
        }
    }

    private static bool IsStable(DateTimeOffset? stableSince, TimeSpan stableFor)
    {
        return stableSince.HasValue && (DateTimeOffset.UtcNow - stableSince.Value) >= stableFor;
    }

    private static void ThrowIfTimedOut(DateTimeOffset started, TimeSpan timeout, string filePath)
    {
        if (DateTimeOffset.UtcNow - started > timeout) throw new TimeoutException($"File '{filePath}' did not become stable within {timeout}.");
    }

    private readonly record struct FileObservation(bool IsAvailable, long SizeBytes)
    {
        public static FileObservation Success(long sizeBytes) => new(true, sizeBytes);
        public static FileObservation Unavailable() => new(false, 0);
    }
}
