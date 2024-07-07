namespace LogMonitor.Processors
{
    public interface IFileNameProcessor
    {
        IAsyncEnumerable<ILogRecord> ProcessFile(string filePath, CancellationToken token);
    }
}
