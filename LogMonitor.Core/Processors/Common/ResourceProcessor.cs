namespace LogMonitor.Core.Processors.Abstract
{
    public abstract class ResourceProcessor : IDisposable
    {
        private readonly List<ResourceProcessor> _nextProcessors = new(1);

        protected readonly IEnumerable<ResourceProcessor> NextProcessors;

        protected readonly ILogger Logger;

        public bool HasNext => _nextProcessors.Any();

        public int ProcessorId { get; }

        public ResourceProcessor(int processorId, ILogger logger)
        {
            ProcessorId = processorId;
            Logger = logger;
            NextProcessors = _nextProcessors.AsEnumerable();
        }

        public void AddNextProcessor(ResourceProcessor? processor)
        {
            if (processor is null) throw new ArgumentNullException(nameof(processor));

            if (GetAllowedNextProcessorTypes().Any(t => t.IsAssignableFrom(processor.GetType())))
            {
                _nextProcessors.Add(processor);
                return;
            }

            throw new InvalidProcessorClassException(ProcessorId, processor.GetType());
        }

        public virtual void Dispose()
        {
            foreach (var processor in _nextProcessors)
            {
                processor.Dispose();
            }

            _nextProcessors.Clear();

            GC.SuppressFinalize(this);
        }

        public abstract IEnumerable<Type> GetAllowedNextProcessorTypes();

        //protected void ThrowIfCancellationRequested(CancellationToken? cancellationToken)
        //{
        //    if (cancellationToken is null) return;

        //    try
        //    {
        //        cancellationToken.Value.ThrowIfCancellationRequested();
        //    }
        //    catch (Exception e)
        //    {
        //        throw new ProcessorException(ProcessorId, e);
        //    }
        //}

        //protected void ThrowProcessorException(string? message = null, Exception? innerException = null) => throw new ProcessorException(ProcessorId, message, innerException);

        //protected void ThrowProcessorException(Exception? innerException) => ThrowProcessorException(null, innerException);

        //protected void ThrowProcessorException(string? message) =>  ThrowProcessorException(message, null);
    }
}
