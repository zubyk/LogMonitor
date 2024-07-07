using System.Text;
using System.Text.Json;
using LogMonitor.Core.Processors.Abstract;

namespace LogMonitor.Processors
{
    public class SambaFileResourceProcessorConfiguration
    {
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public string? FileName { get; set; } = $"Задать имя если используется как {nameof(IStartResourceProcessor)}";
    }

    public class SambaFileResourceProcessorFactory : JsonConfigResourceProcessorFactory<SambaFileResourceProcessor, SambaFileResourceProcessorConfiguration>
    {
        public SambaFileResourceProcessorFactory(JsonSerializerOptions jsonSerializerOptions) : base(jsonSerializerOptions)
        {
        }

        protected override SambaFileResourceProcessor CreateProcessor(int processorId, SambaFileResourceProcessorConfiguration config, ILogger logger)
        {
            return new SambaFileResourceProcessor(processorId, config, logger);
        }
    }
}
