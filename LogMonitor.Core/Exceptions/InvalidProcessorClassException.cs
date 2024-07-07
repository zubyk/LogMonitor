namespace LogMonitor
{
    public class InvalidProcessorClassException : ProcessorException
    {
        public IEnumerable<Type> RequiredTypes { get; }

        public InvalidProcessorClassException(int processorId, params Type[] requiredTypes)
            : base(processorId)
        {
            RequiredTypes = requiredTypes?.ToArray() ?? Enumerable.Empty<Type>();
        }
    }
}
