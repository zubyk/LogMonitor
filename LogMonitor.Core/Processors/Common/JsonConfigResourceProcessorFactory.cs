using System.Text.Json;

namespace LogMonitor.Core.Processors.Abstract
{
    public abstract class JsonConfigResourceProcessorFactory<TResourceProcessor, TResourceConfig> : ResourceProcessorFactory<TResourceProcessor>
        where TResourceProcessor : ResourceProcessor
        where TResourceConfig : class, new()
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        protected JsonConfigResourceProcessorFactory(JsonSerializerOptions jsonSerializerOptions)
        {
            if (jsonSerializerOptions is null) throw new ArgumentNullException(nameof(jsonSerializerOptions));

            _jsonSerializerOptions = new JsonSerializerOptions(jsonSerializerOptions)
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.Strict,
                IncludeFields = false,
                ReadCommentHandling = JsonCommentHandling.Skip,
            };
        }

        sealed public override string GetDefaultConfig()
        {
            var config = new TResourceConfig();

            return JsonSerializer.Serialize(config, _jsonSerializerOptions);
        }

        sealed public override TResourceProcessor CreateProcessor(int processorId, string? config, ILogger logger)
        {
            if (logger is null) throw new ArgumentNullException(nameof(logger));

            if (!ValidateConfig(config, out string? error))
            {
                throw new InvalidProcessorConfigException(processorId, typeof(TResourceConfig), error);
            }

            return CreateProcessor(processorId, JsonSerializer.Deserialize<TResourceConfig>(config, _jsonSerializerOptions), logger);
        }

        protected abstract TResourceProcessor CreateProcessor(int processorId, TResourceConfig config, ILogger logger);

        protected virtual void ValidateConfig(TResourceConfig config)
        {

        }

        sealed public override bool ValidateConfig(string? config, out string? error)
        {
            if (string.IsNullOrWhiteSpace(config))
            {
                error = "Конфигурация не может быть пустая";
                return false;
            }
            else
            {
                error = null;
            }

            try
            {
                if (JsonSerializer.Deserialize<TResourceConfig>(config, _jsonSerializerOptions) is not TResourceConfig configuration)
                {
                    error = "Не удалось создать конфигурацию";
                    return false;
                }

                ValidateConfig(configuration);
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }

            return true;
        }
    }
}