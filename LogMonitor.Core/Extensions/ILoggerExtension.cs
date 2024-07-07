using System.Runtime.CompilerServices;

namespace LogMonitor
{
    public static class ILoggerExtension
    {
        public static IDisposable? BeginScopeFromCaller(this ILogger logger, [CallerMemberName] string? method = null)
        {
            return logger.BeginScope(method ?? string.Empty);
        }

        public static bool FilterAndLogError(this ILogger logger, Exception e, bool filterResult = true)
        {
            logger.LogError(e, "{TryCatchErrorMessage}", e.Message);
            return filterResult;
        }

        public static void LogMethodCall(this ILogger logger, [CallerMemberName] string? method = null, params object[] args)
        {
            if (args?.Length > 0)
            {
                if (logger.IsEnabled(LogLevel.Trace))
                {
                    logger.Log(LogLevel.Trace, AppLogEvents.MethodCall, "Method {method} call with params ({@params})", method, args);
                }
                else
                {
                    logger.Log(LogLevel.Debug, AppLogEvents.MethodCall, "Method {method} call with params ({params})", method, args);
                }
            }
            else
            {
                logger.Log(LogLevel.Debug, AppLogEvents.MethodCall, "Method {method} call", method);
            }
        }
    }
}
