namespace FileTransfer.Kafka.Helpers;

public static class KafkaMessageKey
{
    public static string ForFile(Guid fileId) => fileId.ToString("D");
}
