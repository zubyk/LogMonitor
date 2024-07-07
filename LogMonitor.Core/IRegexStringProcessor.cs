namespace LogMonitor.Processors
{
    public interface IRegexStringProcessor
    {
        IAsyncEnumerable<ILogRecord> ProcessString(string data, CancellationToken token);
    }
}
