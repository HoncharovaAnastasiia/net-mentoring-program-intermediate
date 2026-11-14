namespace FileTransfer.Shared.Generators;

public static class UniquePathGenerator
{
    public static string EnsureUniqueFilePath(string path)
    {
        if (!File.Exists(path)) return path;

        string dir = Path.GetDirectoryName(path)!;
        string name = Path.GetFileNameWithoutExtension(path);
        string ext = Path.GetExtension(path);

        for (int suffixNumber = 1; suffixNumber < int.MaxValue; suffixNumber++)
        {
            var candidate = Path.Combine(dir, $"{name}({suffixNumber}){ext}");
            if (!File.Exists(candidate)) return candidate;
        }

        throw new IOException("Unable to generate unique file name.");
    }
}
