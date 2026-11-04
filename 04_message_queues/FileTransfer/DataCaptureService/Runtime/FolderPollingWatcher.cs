namespace DataCaptureService.Runtime;

public class FolderPollingWatcher
{
    private readonly HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);

    public IEnumerable<string> GetNewFiles(string folderPath, string searchPattern)
    {
        if (!Directory.Exists(folderPath)) yield break;

        foreach (var filePath in Directory.EnumerateFiles(folderPath, searchPattern, SearchOption.TopDirectoryOnly))
        {
            if (seen.Add(filePath)) yield return filePath;
        }
    }

    public void MarkAsProcessed(string filePath)
    {
        seen.Remove(filePath);
    }
}
