using System.Text.Json;
using System.Text.RegularExpressions;
using LogMonitor.Core.Processors.Abstract;

namespace LogMonitor.Processors
{
    public class RegexStringProcessorConfiguration
    {
        public static string DateDayGroupName = "day";
        public static string DateMonthGroupName = "month";
        public static string DateYearGroupName = "year";
        public static string DateHourGroupName = "hour";
        public static string DateMinuteGroupName = "day";
        public static string DateSecondGroupName = "sec";
        public static string DateMSecondGroupName = "msec";
        public static string DateOffsetGroupName = "offset";

        public string DateRegex { get; set; } = "";
        public string InfoLevelRegex { get; set; } = "";
        public string WarningLevelRegex { get; set; } = "";
        public string ErrorLevelRegex { get; set; } = "";
        public string MessageRegex { get; set; } = "";
    }

    public class RegexStringResourceProcessorFactory : JsonConfigResourceProcessorFactory<RegexStringProcessor, RegexStringProcessorConfiguration>
    {
        public RegexStringResourceProcessorFactory(JsonSerializerOptions jsonSerializerOptions) : base(jsonSerializerOptions) 
        { 
        }

        protected override void ValidateConfig(RegexStringProcessorConfiguration config)
        {
            base.ValidateConfig(config);

            if (string.IsNullOrWhiteSpace(config.DateRegex))
            {
                throw new Exception($"Поле {nameof(config.DateRegex)} не может быть пустой стройкой");
            }

            if (!((!string.IsNullOrWhiteSpace(config.InfoLevelRegex) &&  new Regex(config.InfoLevelRegex) is not null)
                | (!string.IsNullOrWhiteSpace(config.WarningLevelRegex) &&  new Regex(config.WarningLevelRegex) is not null)
                | (!string.IsNullOrWhiteSpace(config.ErrorLevelRegex) &&  new Regex(config.ErrorLevelRegex) is not null)))
            {
                throw new Exception($"Хотя бы одно из полей {nameof(config.InfoLevelRegex)}, {nameof(config.WarningLevelRegex)}, {nameof(config.ErrorLevelRegex)} не должно быть пустой стройкой");
            }

            if (string.IsNullOrWhiteSpace(config.MessageRegex))
            {
                throw new Exception($"Поле {nameof(config.MessageRegex)} не может быть пустой стройкой");
            }

            var dateNames = new Regex(config.DateRegex).GetGroupNames();
            
            CheckDateGroupExists(dateNames, RegexStringProcessorConfiguration.DateHourGroupName);
            CheckDateGroupExists(dateNames, RegexStringProcessorConfiguration.DateMinuteGroupName);
            CheckDateGroupExists(dateNames, RegexStringProcessorConfiguration.DateSecondGroupName);

            static void CheckDateGroupExists(string[] dateNames, string groupName)
            {
                if (!dateNames.Contains(groupName)) throw new Exception($"Поле {nameof(config.DateRegex)} должно содержать группу захвата {groupName}");
            }
        }

        protected override RegexStringProcessor CreateProcessor(int processorId, RegexStringProcessorConfiguration config, ILogger logger)
        {
            return new RegexStringProcessor(processorId, config, logger);
        }
    }
}