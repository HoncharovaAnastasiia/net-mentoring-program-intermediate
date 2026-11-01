using Confluent.Kafka;
using FileTransfer.Kafka.Configuration;

namespace FileTransfer.Kafka.Factories;

public class KafkaConsumerFactory
{
    private readonly KafkaOptions kafka;
    private readonly ConsumerOptions consumer;

    public KafkaConsumerFactory(KafkaOptions kafka, ConsumerOptions consumer)
    {
        this.kafka = kafka;
        this.consumer = consumer;
    }

    public IConsumer<string, byte[]> Create()
    {
        return new ConsumerBuilder<string, byte[]>(new ConsumerConfig
        {
            BootstrapServers = kafka.BootstrapServers,
            ClientId = $"{kafka.ClientId}-consumer",
            GroupId = consumer.GroupId,
            EnableAutoCommit = consumer.EnableAutoCommit,
            EnableAutoOffsetStore = consumer.EnableAutoOffsetStore,
            AutoOffsetReset = consumer.AutoOffsetReset,
            SessionTimeoutMs = 30000,
            MaxPollIntervalMs = 600000
        }).Build();
    }
}