using Confluent.Kafka;
using FileTransfer.Kafka.Configuration;

namespace FileTransfer.Kafka.Factories;

public class KafkaProducerFactory
{
    private readonly KafkaOptions _kafka;

    public KafkaProducerFactory(KafkaOptions kafka)
    {
        _kafka = kafka;
    }

    public IProducer<string, byte[]> Create()
    {
        return new ProducerBuilder<string, byte[]>(new ProducerConfig
        {
            BootstrapServers = _kafka.BootstrapServers,
            ClientId = $"{_kafka.ClientId}-producer",
            Acks = Acks.All,
            EnableIdempotence = true,
            LingerMs = 5,
            BatchNumMessages = 1000,
            MessageTimeoutMs = 300000
        }).Build();
    }
}
