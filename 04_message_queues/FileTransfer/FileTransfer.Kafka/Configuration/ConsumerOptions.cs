using Confluent.Kafka;

namespace FileTransfer.Kafka.Configuration;

public record ConsumerOptions
{
    public required string GroupId { get; init; }

    public bool EnableAutoCommit { get; init; } = false;
    
    public bool EnableAutoOffsetStore { get; init; } = false;
    
    public AutoOffsetReset AutoOffsetReset { get; init; } = AutoOffsetReset.Earliest;
}
