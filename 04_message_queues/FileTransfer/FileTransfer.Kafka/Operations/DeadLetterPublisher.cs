using Confluent.Kafka;
using FileTransfer.Domain.Messages;
using FileTransfer.Kafka.Helpers;
using FileTransfer.Shared.Helpers;

namespace FileTransfer.Kafka.Operations;

public class DeadLetterPublisher
{
    private readonly IProducer<string, byte[]> producer;
    private readonly string deadLetterTopic;

    public DeadLetterPublisher(IProducer<string, byte[]> producer, string deadLetterTopic)
    {
        this.producer = producer;
        this.deadLetterTopic = deadLetterTopic;
    }

    public Task PublishAsync(DeadLetterRecord record, CancellationToken ct = default)
    {
        var key = record.FileId.HasValue ? KafkaMessageKey.ForFile(record.FileId.Value) : string.Empty;
        var value = JsonMessageSerializer.SerializeToUtf8Bytes(record);

        return producer.ProduceAsync(deadLetterTopic, new Message<string, byte[]> { Key = key, Value = value }, ct);
    }
}
