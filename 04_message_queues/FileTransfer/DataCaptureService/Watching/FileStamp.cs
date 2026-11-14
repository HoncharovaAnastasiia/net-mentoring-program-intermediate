namespace DataCaptureService.Watching;

public readonly record struct FileStamp(long Length, DateTime LastWriteTimeUtc);
