using System.Diagnostics.CodeAnalysis;

namespace DataCaptureService.Watching;

public class FolderPollingWatcher
{
    private readonly Dictionary<string, FileStamp> seen = new(StringComparer.OrdinalIgnoreCase);

    public IEnumerable<string> GetNewFiles(string folderPath, string searchPattern)
    {
        if (!Directory.Exists(folderPath)) yield break;

        foreach (var filePath in Directory.EnumerateFiles(folderPath, searchPattern, SearchOption.TopDirectoryOnly))
        {
            var stamp = TryGetStamp(filePath);

            if (IsFileNew(stamp, filePath))
            {
                seen[filePath] = stamp.Value;
                yield return filePath;
            }
        }
    }

    private static FileStamp? TryGetStamp(string filePath)
    {
        try
        {
            var fileInfo = new FileInfo(filePath);
            return new FileStamp(fileInfo.Length, fileInfo.LastWriteTimeUtc);
        }
        catch
        {
            return null;
        }
    }

    private bool IsFileNew([NotNullWhen(true)] FileStamp? stamp, string filePath)
    {
        return stamp is not null && (!seen.TryGetValue(filePath, out var oldStamp) || !oldStamp.Equals(stamp.Value));
    }
}
