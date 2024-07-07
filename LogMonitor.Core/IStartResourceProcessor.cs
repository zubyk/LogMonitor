namespace LogMonitor.Processors
{
    public interface IStartResourceProcessor
    {
        IAsyncEnumerable<ILogRecord> Start(CancellationToken token);
    }
}
