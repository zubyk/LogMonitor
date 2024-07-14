namespace LogMonitor
{
    partial class ILoggerExtension
    {
        private static readonly int _infoEventIdShift = 16000;
        private static readonly int _warningEventIdShift = _infoEventIdShift + 1000;
        private static readonly int _errorEventIdShift = _warningEventIdShift + 1000;

        public static readonly EventId MethodCall = new(_infoEventIdShift + 1, nameof(MethodCall));
        public static readonly EventId ExecutionError = new(_errorEventIdShift + 1, nameof(ExecutionError));
    }
}
