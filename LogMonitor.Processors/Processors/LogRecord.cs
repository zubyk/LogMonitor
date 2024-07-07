namespace LogMonitor.Processors
{
    public class LogRecord : ILogRecord
    {
        public string RawText { get; }

        public DateTimeOffset Date { get; set; }

        public LogLevel Level { get; set; } = LogLevel.None;

        public string Message { get; set; }

        public string Source { get; set; }

        public string MessageKey { get; set; }

        public LogRecord(string rawText)
        {
            RawText = rawText;
        }
    }
}