using System.Runtime.CompilerServices;
using System.Text;
using LogMonitor.Core.Processors.Abstract;

namespace LogMonitor.Processors
{
    public class SambaFileResourceProcessor : ResourceProcessor, IFileNameProcessor, IStartResourceProcessor
    {
        protected readonly Encoding FileEncoding;
        protected readonly string? FileName;

        internal SambaFileResourceProcessor(int processorId, SambaFileResourceProcessorConfiguration config, ILogger logger) : base(processorId, logger)
        {
            FileEncoding = config.Encoding;
            FileName = config.FileName;
        }

        public override IEnumerable<Type> GetAllowedNextProcessorTypes()
        {
            return new[] { 
                typeof(IRegexStringProcessor),
                typeof(IFileNameProcessor),
            }.AsEnumerable();
        }

        async IAsyncEnumerable<ILogRecord> IStartResourceProcessor.Start([EnumeratorCancellation] CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                yield break;
            }

            using (Logger.BeginScopeFromCaller())
            {
                Logger.LogMethodCall();

                if (string.IsNullOrWhiteSpace(FileName))
                {
                    Logger.LogWarning("FileName {file} is empty", FileName);
                    yield break;
                }

                await foreach (var result in (this as IFileNameProcessor).ProcessFile(FileName, token))
                {
                    yield return result;
                }
            }
        }

        async IAsyncEnumerable<ILogRecord> IFileNameProcessor.ProcessFile(string filePath, [EnumeratorCancellation] CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                yield break;
            }

            using (Logger.BeginScopeFromCaller())
            {
                Logger.LogMethodCall(args: filePath);

                if (!File.Exists(filePath))
                {
                    Logger.LogWarning("File {file} not exists", filePath);
                    yield break;
                }
                else if (!HasNext)
                {
                    Logger.LogWarning("Has no next processors");
                    yield break;
                }
                else
                {
                    using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                    {
                        Logger.LogInformation("File {file} opened", filePath);

                        using (var reader = new StreamReader(file, FileEncoding, false, 512))
                        {
                            ulong lineNumber = 1;

                            Logger.LogDebug("Reading {lineNum} line from {file}", lineNumber++, filePath);

                            while (!token.IsCancellationRequested && await reader.ReadLineAsync(token) is string line)
                            {
                                foreach (IRegexStringProcessor processor in NextProcessors.Cast<IRegexStringProcessor>())
                                {
                                    if (token.IsCancellationRequested)
                                    {
                                        yield break;
                                    }

                                    await foreach (var subResult in processor.ProcessString(line, token))
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
        }
    }
}
