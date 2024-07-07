using System.Text.Json;
using LogMonitor.Core.Processors.Abstract;

namespace LogMonitor.Processors
{
    public class StartStringResourceProcessorConfiguration
    {
        public string StartString { get; set; } = "Введите начальное значение";
    }

    public class StartStringResourceProcessorFactory : JsonConfigResourceProcessorFactory<StartStringResourceProcessor, StartStringResourceProcessorConfiguration>
    {
        public StartStringResourceProcessorFactory(JsonSerializerOptions jsonSerializerOptions) : base(jsonSerializerOptions) 
        { 
        }

        protected override void ValidateConfig(StartStringResourceProcessorConfiguration config)
        {
            base.ValidateConfig(config);

            if (string.IsNullOrWhiteSpace(config.StartString))
            {
                throw new Exception($"Поле {nameof(config.StartString)} не может быть пустой стройкой");
            }
        }

        protected override StartStringResourceProcessor CreateProcessor(int processorId, StartStringResourceProcessorConfiguration config, ILogger logger)
        {
            return new StartStringResourceProcessor(processorId, config, logger);
        }
    }
}