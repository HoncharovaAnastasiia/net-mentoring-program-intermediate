namespace ProcessingService.Configuration;

public record ProcessingOptions
{
    public required string WorkFolder { get; init; }

    public required string OutputFolder { get; init; }
    
    public string? QuarantineFolder { get; init; }

    public TimeSpan TransferTimeout { get; init; } = TimeSpan.FromHours(1);
}
