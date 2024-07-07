namespace LogMonitor
{
    public class InvalidProcessorConfigException : ProcessorException
    {
        public Type ConfigType { get; }

        public InvalidProcessorConfigException(int processorId, Type configType, string? error)
            : base(processorId, error)
        {
            ConfigType = configType;
        }
    }
}
