namespace LogMonitor.Processors
{
    public interface ILogRecord
    {
        public DateTimeOffset Date { get; set; }
        
        public LogLevel Level { get; set; }

        public string Message { get; set; }

        public string RawText { get; }

        public string Source { get; set; }

        public string MessageKey { get; set; }
    }
}