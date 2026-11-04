namespace DataCaptureService.Configuration;

public record DataCaptureOptions
{
    public required string InputFolder { get; init; }
    
    public string? SentFolder { get; init; }

    public string SearchPattern { get; init; } = "*.pdf";

    public int ChunkSizeBytes { get; init; } = 1024 * 1024;

    public TimeSpan StableFor { get; init; } = TimeSpan.FromSeconds(2);
    
    public TimeSpan PollInterval { get; init; } = TimeSpan.FromSeconds(1);
    
    public TimeSpan StabilityTimeout { get; init; } = TimeSpan.FromMinutes(2);

    public TimeSpan ScanInterval { get; init; } = TimeSpan.FromSeconds(1);
}
