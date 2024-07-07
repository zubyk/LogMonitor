using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using LogMonitor.Core.Processors.Abstract;

namespace LogMonitor.Processors
{
    public class RegexStringProcessor : ResourceProcessor, IRegexStringProcessor
    {
        protected readonly Regex DateRegex;
        protected readonly Regex MessageRegex;
        protected readonly Regex? InfoLevelRegex;
        protected readonly Regex? WarningLevelRegex;
        protected readonly Regex? ErrorLevelRegex;

        internal RegexStringProcessor(int processorId, RegexStringProcessorConfiguration config, ILogger logger) : base(processorId, logger)
        {
            DateRegex = new Regex(config.DateRegex, RegexOptions.Singleline | RegexOptions.Compiled);
            MessageRegex = new Regex(config.MessageRegex, RegexOptions.Singleline | RegexOptions.Compiled);

            InfoLevelRegex = CreateIfExists(config.InfoLevelRegex);
            WarningLevelRegex = CreateIfExists(config.InfoLevelRegex);
            ErrorLevelRegex = CreateIfExists(config.InfoLevelRegex);

            static Regex? CreateIfExists(string? regexString)
            {
                if (!string.IsNullOrWhiteSpace(regexString))
                {
                    return new Regex(regexString, RegexOptions.Singleline | RegexOptions.Compiled);
                }
                else
                {
                    return null;
                }
            }
        }

        public override IEnumerable<Type> GetAllowedNextProcessorTypes()
        {
            return new[] { typeof(ILogRecordProcessor) }.AsEnumerable();
        }

        public async IAsyncEnumerable<ILogRecord> ProcessString(string data, [EnumeratorCancellation] CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                yield break;
            }

            using (Logger.BeginScopeFromCaller())
            {
                Logger.LogMethodCall(args: data);
                
                if (string.IsNullOrWhiteSpace(data))
                {
                    Logger.LogWarning("Try processing empty string");
                    yield break;
                }
                else 
                {
                    ILogRecord logRecord = new LogRecord(data);

                    logRecord.Level = GetLogLevel(data);


                    if (!HasNext)
                    {
                        yield return logRecord;
                    }
                    else
                    {
                        foreach (var processor in NextProcessors.Cast<ILogRecordProcessor>())
                        {
                            if (token.IsCancellationRequested)
                            {
                                yield break;
                            }

                            await foreach (var subResult in processor.ProcessRecord(logRecord, token))
                            {
                                if (subResult is not null)
                                {
                                    yield return subResult;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected LogLevel GetLogLevel(string processedString)
        {
            if (ErrorLevelRegex is not null)
            {
                if (ErrorLevelRegex.IsMatch(processedString)) 
                { 
                    return LogLevel.Error;
                }
            }
            else if (WarningLevelRegex is not null)
            {
                if (WarningLevelRegex.IsMatch(processedString))
                {
                    return LogLevel.Warning;
                }
            }
            else
            {
                if (InfoLevelRegex.IsMatch(processedString))
                {
                    return LogLevel.Information;
                }
            }

            return LogLevel.None;
        }

        protected string GetLogMessage(string processedString)
        {
            return MessageRegex.Match(processedString)?.Value ?? string.Empty;
        }

        protected DateTimeOffset GetLogDate(string processedString)
        {
            var match = DateRegex.Match(processedString);

            var hour = int.Parse(match.Groups[RegexStringProcessorConfiguration.DateHourGroupName].Value);
            var min = int.Parse(match.Groups[RegexStringProcessorConfiguration.DateMinuteGroupName].Value);
            var sec = int.Parse(match.Groups[RegexStringProcessorConfiguration.DateSecondGroupName].Value);

            if (!GetGroupValue(match, RegexStringProcessorConfiguration.DateMSecondGroupName, out int msec))
            {
                msec = 0;
            }

            var now = DateTimeOffset.Now;

            if (!GetGroupValue(match, RegexStringProcessorConfiguration.DateDayGroupName, out int day))
            {
                day = now.Day;
            }
            if (!GetGroupValue(match, RegexStringProcessorConfiguration.DateMonthGroupName, out int month))
            {
                month = now.Month;
            }
            if (!GetGroupValue(match, RegexStringProcessorConfiguration.DateYearGroupName, out int year))
            {
                year = now.Year;
            }

            var offset = GetGroupValue(match, RegexStringProcessorConfiguration.DateYearGroupName, out int off) ? TimeSpan.FromHours((double)off) : TimeSpan.Zero;
            
            return new DateTimeOffset(year, month, day, hour, min, sec, msec, offset);

            static bool GetGroupValue(Match match, string groupName, out int value)
            {
                if (match.Groups[groupName] is Group g && g.Success)
                {
                    return int.TryParse(g.Value, out value);
                }
                value = 0;
                return false;

            }
        }
    }
}
