namespace LogMonitor
{
    public abstract class ProcessorException : Exception
    {
        public int ProcessorId { get; }
                
        public ProcessorException(int processorId, string? message = null, Exception? innerException = null) 
            : base(message ?? innerException?.Message, innerException)
        {
            ProcessorId = processorId;
        }

        public ProcessorException(int processorId, Exception? innerException)
           : this(processorId, message: innerException?.Message, innerException: innerException)
        {
        }

        public ProcessorException(int processorId, string? message)
           : this(processorId, message: message, innerException: null)
        {
        }
    }
}
