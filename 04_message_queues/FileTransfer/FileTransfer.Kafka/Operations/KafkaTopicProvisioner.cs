using Confluent.Kafka;
using Confluent.Kafka.Admin;
using FileTransfer.Kafka.Configuration;

namespace FileTransfer.Kafka.Operations;

public class KafkaTopicProvisioner
{
    private readonly KafkaOptions kafka;
    private readonly TopicOptions topics;

    public KafkaTopicProvisioner(KafkaOptions kafka, TopicOptions topics)
    {
        this.kafka = kafka;
        this.topics = topics;
    }

    public async Task EnsureTopicsExistAsync(CancellationToken ct = default)
    {
        using var admin = CreateAdminClient();
        var specs = BuildTopicSpecifications();

        try
        {
            await admin.CreateTopicsAsync(specs);
        }
        catch (CreateTopicsException ex) when (AllAlreadyExist(ex)) { }
    }

    private IAdminClient CreateAdminClient()
    {
        return new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = kafka.BootstrapServers,
            ClientId = $"{kafka.ClientId}-admin"
        }).Build();
    }

    private IEnumerable<TopicSpecification> BuildTopicSpecifications()
    {
        yield return NewTopic(topics.ManifestTopic);
        yield return NewTopic(topics.ChunksTopic);
        yield return NewTopic(topics.DeadLetterTopic);
    }

    private TopicSpecification NewTopic(string name) => new()
    {
        Name = name,
        NumPartitions = topics.NumPartitions,
        ReplicationFactor = topics.ReplicationFactor
    };

    private static bool AllAlreadyExist(CreateTopicsException ex)
    {
        return ex.Results.Count > 0 && ex.Results.All(r => r.Error.Code == ErrorCode.TopicAlreadyExists);
    }
}
