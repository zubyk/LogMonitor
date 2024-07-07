namespace LogMonitor.Processors
{
    public interface ILogRecordProcessor
    {
        IAsyncEnumerable<ILogRecord> ProcessRecord(ILogRecord logRecord, CancellationToken token);
    }
}
