using DataCaptureService.Configuration;
using DataCaptureService.Runtime;
using FileTransfer.Domain.Constants;
using FileTransfer.Kafka.Configuration;
using FileTransfer.Kafka.Factories;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true).AddEnvironmentVariables().Build();

var kafka = configuration.GetSection("Kafka").Get<KafkaOptions>() ?? throw new InvalidOperationException("Kafka configuration is missing.");
var options = configuration.GetSection("DataCapture").Get<DataCaptureOptions>() ?? throw new InvalidOperationException("DataCapture configuration is missing.");

string manifestTopic = configuration["Topics:ManifestTopic"] ?? TopicNames.Manifest;
string chunksTopic = configuration["Topics:ChunksTopic"] ?? TopicNames.Chunks;

using var producer = new KafkaProducerFactory(kafka).Create();

var sender = new FileSender(producer, manifestTopic, chunksTopic, Environment.MachineName);
var watcher = new FolderPollingWatcher();
var worker = new CaptureWorker(options, watcher, sender);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

await worker.RunAsync(cts.Token);