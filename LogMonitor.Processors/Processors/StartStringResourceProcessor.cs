using System.Runtime.CompilerServices;
using LogMonitor.Core.Processors.Abstract;

namespace LogMonitor.Processors
{
    public class StartStringResourceProcessor : ResourceProcessor, IStartResourceProcessor
    {
        protected readonly string StartString;

        internal StartStringResourceProcessor(int processorId, StartStringResourceProcessorConfiguration config, ILogger logger) : base(processorId, logger)
        {
            StartString = config.StartString;
        }

        public override IEnumerable<Type> GetAllowedNextProcessorTypes()
        {
            return new[] { typeof(IFileNameProcessor) }.AsEnumerable();
        }

        async IAsyncEnumerable<ILogRecord> IStartResourceProcessor.Start([EnumeratorCancellation] CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                yield break;
            }

            using (Logger.BeginScopeFromCaller())
            {
                if (!HasNext)
                {
                    Logger.LogWarning("Has no next processors");
                    yield break;
                }
                else
                {
                    foreach (var processor in NextProcessors.Cast<IFileNameProcessor>())
                    {
                        if (token.IsCancellationRequested)
                        {
                            yield break;
                        }

                        await foreach (var subResult in processor.ProcessFile(StartString, token))
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
}
