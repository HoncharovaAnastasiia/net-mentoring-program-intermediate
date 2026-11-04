using FileTransfer.Domain.Messages;
using System.Collections.Concurrent;

namespace ProcessingService.Transfers;

public class TransferSessionStore
{
    private readonly ConcurrentDictionary<Guid, TransferSession> sessions = new();

    public bool TryGet(Guid fileId, out TransferSession session) => sessions.TryGetValue(fileId, out session!);

    public TransferSession GetOrCreate(FileManifest manifest, Func<FileManifest, TransferSession> factory)
    {
        return sessions.GetOrAdd(manifest.FileId, _ => factory(manifest));
    }

    public bool TryRemove(Guid fileId, out TransferSession session) => sessions.TryRemove(fileId, out session!);

    public IEnumerable<TransferSession> GetAll() => sessions.Values;
}
