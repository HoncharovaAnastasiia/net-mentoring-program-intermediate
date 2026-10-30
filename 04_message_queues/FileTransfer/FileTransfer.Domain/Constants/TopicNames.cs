namespace FileTransfer.Domain.Constants;

public static class TopicNames
{
    public const string Manifest = "file-transfer-manifest";
    public const string Chunks = "file-transfer-chunks";
    public const string DeadLetter = "file-transfer-deadletter";
}
