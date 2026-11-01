namespace FileTransfer.Kafka.Configuration;

public record KafkaOptions
{
    public required string BootstrapServers { get; init; }

    public string ClientId { get; init; } = "file-transfer";
}