using FileTransfer.Kafka.Configuration;
using FileTransfer.Kafka.Factories;
using FileTransfer.Kafka.Operations;
using Microsoft.Extensions.Configuration;
using ProcessingService.Configuration;
using ProcessingService.Handlers;
using ProcessingService.Runtime;
using ProcessingService.Transfers;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true).AddEnvironmentVariables().Build();

var kafka = configuration.GetSection("Kafka").Get<KafkaOptions>() ?? throw new InvalidOperationException("Kafka configuration is missing.");
var consumerOptions = configuration.GetSection("KafkaConsumer").Get<ConsumerOptions>() ?? throw new InvalidOperationException("KafkaConsumer configuration is missing.");
var topics = configuration.GetSection("Topics").Get<TopicOptions>() ?? throw new InvalidOperationException("Topics configuration is missing.");
var processing = configuration.GetSection("Processing").Get<ProcessingOptions>() ?? throw new InvalidOperationException("Processing configuration is missing.");

await new KafkaTopicProvisioner(kafka, topics).EnsureTopicsExistAsync();

using var producer = new KafkaProducerFactory(kafka).Create();

var deadLetterPublisher = new DeadLetterPublisher(producer, topics.DeadLetterTopic);
var deadLetterHandler = new DeadLetterHandler(deadLetterPublisher, topics.ManifestTopic, topics.ChunksTopic);

using var consumer = new KafkaConsumerFactory(kafka, consumerOptions).Create();

var store = new TransferSessionStore();
var worker = new ProcessingWorker(consumer, processing, store, deadLetterHandler, topics.ManifestTopic, topics.ChunksTopic);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, cancelEvent) => { cancelEvent.Cancel = true; cts.Cancel(); };

await worker.RunAsync(cts.Token);