using Confluent.Kafka;
using FileTransfer.Domain.Messages;
using FileTransfer.Kafka.Operations;
using FileTransfer.Shared.Helpers;
using System.Text;

namespace ProcessingService.Handlers;

public class DeadLetterHandler
{
    private readonly DeadLetterPublisher publisher;
    private readonly string manifestTopic;
    private readonly string chunksTopic;

    public DeadLetterHandler(DeadLetterPublisher publisher, string manifestTopic, string chunksTopic)
    {
        this.publisher = publisher;
        this.manifestTopic = manifestTopic;
        this.chunksTopic = chunksTopic;
    }

    public Task PublishAsync(ConsumeResult<string, byte[]> result, Exception ex)
    {
        return publisher.PublishAsync(new DeadLetterRecord(
            result.Topic,
            result.Partition.Value,
            result.Offset.Value,
            $"{ex.GetType().Name}: {ex.Message}",
            TryExtractFileId(result),
            TryExtractChunkIndex(result),
            TryGetUtf8Text(result.Message.Value)));
    }

    private Guid? TryExtractFileId(ConsumeResult<string, byte[]> result)
    {
        try
        {
            if (result.Topic == manifestTopic)
            {
                return JsonMessageSerializer.Deserialize<FileManifest>(result.Message.Value).FileId;
            }

            if (result.Topic == chunksTopic)
            {
                return JsonMessageSerializer.Deserialize<FileChunk>(result.Message.Value).FileId;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private int? TryExtractChunkIndex(ConsumeResult<string, byte[]> result)
    {
        try
        {
            if (result.Topic == chunksTopic)
            {
                return JsonMessageSerializer.Deserialize<FileChunk>(result.Message.Value).ChunkIndex;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static string? TryGetUtf8Text(byte[] bytes)
    {
        try { return Encoding.UTF8.GetString(bytes); }
        catch { return null; }
    }
}
