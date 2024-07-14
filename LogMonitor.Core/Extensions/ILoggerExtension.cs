using System.Runtime.CompilerServices;

namespace LogMonitor
{
    public static partial class ILoggerExtension
    {
        public static IDisposable? BeginScopeFromCaller(this ILogger logger, object? caller = null, [CallerMemberName] string? method = null)
        {
            if (caller is string str)
            {
                return logger.BeginScope("{caller}.{method}", str, method);
            }
            else if (caller is not null)
            {
                return logger.BeginScope("{caller}.{method}", caller.GetType().Name, method);
            }

            return logger.BeginScope(method ?? string.Empty);
        }

        public static bool FilterAndLogError(this ILogger logger, LogLevel logLevel, Exception e, bool filterResult = true)
        {
            logger.LogExecutionError(logLevel, e.Message, e);
            return filterResult;
        }

        public static bool FilterAndLogError(this ILogger logger, Exception e, bool filterResult = true)
            => FilterAndLogError(logger, LogLevel.Error, e, filterResult);

        public static void LogMethodCall(this ILogger logger, object? caller = null, [CallerMemberName] string? method = null, params object[] args)
        {
            if (caller is string str)
            {
                CallEvent(str);
            }
            else if (caller is not null)
            {
                CallEvent(caller.GetType().Name);
            }
            else
            {
                CallEvent(string.Empty);
            }

            void CallEvent(string callerName)
            {
                if (args?.Length > 0)
                {
                    _methodCallWithParamsEvent(logger, callerName, method, args, null);
                }
                else
                {
                    _methodCallEvent(logger, callerName, method, null);
                }
            }
        }
    }
}
