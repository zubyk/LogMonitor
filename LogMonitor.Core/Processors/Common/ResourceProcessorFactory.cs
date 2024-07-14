﻿namespace LogMonitor.Core.Processors.Abstract
{
    public abstract class ResourceProcessorFactory<TResourceProcessor> where TResourceProcessor : ResourceProcessor
    {
        public virtual bool HasConfig => false;

        public virtual string? GetDefaultConfig() => null;

        public abstract bool ValidateConfig(string? config, out string? error);

        public abstract TResourceProcessor CreateProcessor(int processorId, string config, ILogger logger);
    }
}
