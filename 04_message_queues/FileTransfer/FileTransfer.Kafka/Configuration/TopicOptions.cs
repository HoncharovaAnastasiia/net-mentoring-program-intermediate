namespace FileTransfer.Kafka.Configuration;

public record TopicOptions
{
    public required string ManifestTopic { get; init; }

    public required string ChunksTopic { get; init; }
    
    public required string DeadLetterTopic { get; init; }

    public int NumPartitions { get; init; } = 6;
    
    public short ReplicationFactor { get; init; } = 1;
}
