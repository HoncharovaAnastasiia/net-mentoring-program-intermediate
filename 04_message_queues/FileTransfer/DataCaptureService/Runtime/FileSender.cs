using Confluent.Kafka;
using FileTransfer.Domain.Messages;
using FileTransfer.Kafka.Helpers;
using FileTransfer.Shared.Helpers;
using System.Security.Cryptography;

namespace DataCaptureService.Runtime;

public sealed class FileSender
{
    private readonly IProducer<string, byte[]> producer;
    private readonly string manifestTopic;
    private readonly string chunksTopic;
    private readonly string sourceInstanceId;

    public FileSender(IProducer<string, byte[]> producer, string manifestTopic, string chunksTopic, string sourceInstanceId)
    {
        this.producer = producer;
        this.manifestTopic = manifestTopic;
        this.chunksTopic = chunksTopic;
        this.sourceInstanceId = sourceInstanceId;
    }

    public async Task SendAsync(string filePath, int chunkSizeBytes, TimeSpan stableFor, TimeSpan stabilityPollInterval, TimeSpan stabilityTimeout, CancellationToken ct)
    {
        await FileStabilityChecker.WaitUntilStableAsync(filePath, stableFor, stabilityPollInterval, stabilityTimeout, ct);

        var fileInfo = new FileInfo(filePath);
        var fileId = Guid.NewGuid();
        var key = KafkaMessageKey.ForFile(fileId);

        var totalBytes = fileInfo.Length;
        var totalChunks = ChunkCalculator.GetTotalChunks(totalBytes, chunkSizeBytes);

        var fileSha256Hex = await ComputeFileSha256HexAsync(filePath, ct);

        var manifest = CreateManifest(fileId, fileInfo.Name, totalBytes, chunkSizeBytes, totalChunks, fileSha256Hex);

        await ProduceAsync(manifestTopic, key, manifest, ct);
        await ProduceChunksAsync(filePath, fileId, key, totalChunks, chunkSizeBytes, ct);

        producer.Flush(TimeSpan.FromSeconds(30));
    }

    private FileManifest CreateManifest(Guid fileId, string originalFileName, long totalBytes, int chunkSizeBytes, int totalChunks, string fileSha256Hex)
    {
        return new FileManifest
        {
            FileId = fileId,
            OriginalFileName = originalFileName,
            TotalBytes = totalBytes,
            ChunkSizeBytes = chunkSizeBytes,
            TotalChunks = totalChunks,
            FileSha256Hex = fileSha256Hex,
            SourceInstanceId = sourceInstanceId
        };
    }

    private Task<DeliveryResult<string, byte[]>> ProduceAsync<T>(string topic, string key, T message, CancellationToken ct)
    {
        return producer.ProduceAsync(topic, new Message<string, byte[]>
        {
            Key = key,
            Value = JsonMessageSerializer.SerializeToUtf8Bytes(message)
        }, ct);
    }

    private async Task ProduceChunksAsync(string filePath, Guid fileId, string key, int totalChunks, int chunkSizeBytes, CancellationToken ct)
    {
        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var buffer = new byte[chunkSizeBytes];
        var chunkIndex = 0;

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            var read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), ct);
            if (read == 0) break;

            var payload = new byte[read];
            Buffer.BlockCopy(buffer, 0, payload, 0, read);

            await ProduceAsync(chunksTopic, key, new FileChunk
            {
                FileId = fileId,
                ChunkIndex = chunkIndex,
                TotalChunks = totalChunks,
                ChunkSha256Hex = ComputeSha256Hex(payload),
                ChunkBytes = payload
            }, ct);
            chunkIndex++;
        }

        if (chunkIndex != totalChunks)
        {
            throw new InvalidOperationException($"Chunk count mismatch. Expected {totalChunks}, produced {chunkIndex}.");
        }
    }

    private static string ComputeSha256Hex(ReadOnlySpan<byte> bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

    private static async Task<string> ComputeFileSha256HexAsync(string filePath, CancellationToken ct)
    {
        await using var stream = File.OpenRead(filePath);
        var hash = await SHA256.HashDataAsync(stream, ct);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
