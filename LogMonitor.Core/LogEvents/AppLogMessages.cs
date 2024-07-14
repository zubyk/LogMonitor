namespace LogMonitor
{
    partial class ILoggerExtension
    {
        private static readonly Action<ILogger, string, string?, Exception?>
       _methodCallEvent = LoggerMessage.Define<string, string?>(
           LogLevel.Debug,
           MethodCall,
           "Method {caller}.{method}() call");

        private static readonly Action<ILogger, string, string?, object[], Exception?>
        _methodCallWithParamsEvent = LoggerMessage.Define<string, string?, object[]>(
            LogLevel.Debug,
            MethodCall,
            "Method {caller}.{method}() call with params ({args})");

        [LoggerMessage(
            EventId = 16000 + 1000 + 1000 + 1,
            EventName = nameof(ExecutionError),
            Message = "{error}")]
        public static partial void LogExecutionError(this ILogger logger, LogLevel logLevel, string error, Exception exception);
    }
}
