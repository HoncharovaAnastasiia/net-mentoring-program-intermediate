using Confluent.Kafka;
using FileTransfer.Domain.Messages;
using FileTransfer.Shared.Helpers;
using ProcessingService.Configuration;
using ProcessingService.Handlers;
using ProcessingService.Transfers;
using ProcessingService.Validators;

namespace ProcessingService.Runtime;

public class ProcessingWorker
{
    private readonly IConsumer<string, byte[]> consumer;
    private readonly ProcessingOptions options;
    private readonly TransferSessionStore store;
    private readonly DeadLetterHandler deadLetterHandler;
    private readonly string manifestTopic;
    private readonly string chunksTopic;

    public ProcessingWorker(
        IConsumer<string, byte[]> consumer,
        ProcessingOptions options,
        TransferSessionStore store,
        DeadLetterHandler deadLetterHandler,
        string manifestTopic,
        string chunksTopic)
    {
        this.consumer = consumer;
        this.options = options;
        this.store = store;
        this.deadLetterHandler = deadLetterHandler;
        this.manifestTopic = manifestTopic;
        this.chunksTopic = chunksTopic;
    }

    public Task RunAsync(CancellationToken ct)
    {
        Directory.CreateDirectory(options.WorkFolder);
        Directory.CreateDirectory(options.OutputFolder);

        if (!string.IsNullOrWhiteSpace(options.QuarantineFolder))
        {
            Directory.CreateDirectory(options.QuarantineFolder);
        }

        consumer.Subscribe([manifestTopic, chunksTopic]);

        return ConsumeLoopAsync(ct);
    }

    private async Task ConsumeLoopAsync(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                ConsumeResult<string, byte[]> result;

                try
                {
                    result = await Task.Run(() => consumer.Consume(ct), ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                await HandleAndCommitAsync(result, ct);
            }
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task HandleAndCommitAsync(ConsumeResult<string, byte[]> result, CancellationToken ct)
    {
        try
        {
            await ProcessMessage(result, ct);
        }
        catch (Exception ex)
        {
            try { await deadLetterHandler.PublishAsync(result, ex); }
            catch { }
        }
        finally
        {
            consumer.StoreOffset(result);
            consumer.Commit();
        }
    }

    private async Task ProcessMessage(ConsumeResult<string, byte[]> result, CancellationToken ct)
    {
        if (result.Topic == manifestTopic)
        {
            var manifest = JsonMessageSerializer.Deserialize<FileManifest>(result.Message.Value);
            await HandleManifestAsync(manifest);
            return;
        }

        if (result.Topic == chunksTopic)
        {
            var chunk = JsonMessageSerializer.Deserialize<FileChunk>(result.Message.Value);
            await HandleChunkAsync(chunk, ct);
            return;
        }

        throw new InvalidOperationException($"Unexpected topic: {result.Topic}");
    }

    private Task HandleManifestAsync(FileManifest manifest)
    {
        MessageValidator.ValidateManifest(manifest);

        store.GetOrCreate(manifest, m =>
        {
            var tempPath = Path.Combine(options.WorkFolder, $"{m.FileId:D}.tmp");
            FileReassembler.EnsureTempFile(tempPath, m.TotalBytes);
            return new TransferSession(m, tempPath);
        });

        return Task.CompletedTask;
    }

    private async Task HandleChunkAsync(FileChunk chunk, CancellationToken ct)
    {
        MessageValidator.ValidateChunkBasics(chunk);

        if (!store.TryGet(chunk.FileId, out var session))
        {
            throw new InvalidOperationException($"Unknown FileId '{chunk.FileId:D}' (manifest not received yet).");
        }

        if (chunk.ChunkIndex < 0 || chunk.ChunkIndex >= session.Manifest.TotalChunks)
        {
            throw new InvalidOperationException($"ChunkIndex {chunk.ChunkIndex} is out of range.");
        }

        MessageValidator.ValidateChunkIndex(chunk, session.Manifest);
        MessageValidator.EnsureChunkHashValid(chunk);

        if (session.ReceivedChunks[chunk.ChunkIndex]) return;

        await FileReassembler.WriteChunkAsync(session.TempFilePath, chunk, session.Manifest.ChunkSizeBytes, ct);
        session.MarkChunkReceived(chunk.ChunkIndex);

        if (session.IsComplete()) await FinalizeAsync(session, ct);
    }

    private async Task FinalizeAsync(TransferSession session, CancellationToken ct)
    {
        var actualSha = await FileReassembler.ComputeFileSha256HexAsync(session.TempFilePath, ct);
        if (!string.Equals(actualSha, session.Manifest.FileSha256Hex, StringComparison.OrdinalIgnoreCase))
        {
            MoveToQuarantine(session.TempFilePath, session.Manifest.FileId);
            store.TryRemove(session.Manifest.FileId, out _);

            throw new InvalidOperationException("Final file hash mismatch.");
        }

        FileReassembler.MoveToOutput(session.TempFilePath, options.OutputFolder, session.Manifest.OriginalFileName);
        store.TryRemove(session.Manifest.FileId, out _);
    }

    private void MoveToQuarantine(string tempFilePath, Guid fileId)
    {
        if (string.IsNullOrWhiteSpace(options.QuarantineFolder)) return;

        Directory.CreateDirectory(options.QuarantineFolder);

        var target = Path.Combine(options.QuarantineFolder, $"{fileId:D}.tmp");
        if (File.Exists(target)) File.Delete(target);

        File.Move(tempFilePath, target);
    }
}
