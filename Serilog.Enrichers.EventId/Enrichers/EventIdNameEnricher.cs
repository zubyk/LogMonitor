using Serilog.Core;
using Serilog.Events;
using System;
using System.Linq;

namespace Serilog.Enrichers
{
    public class EventIdNameEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Properties.TryGetValue("EventId", out LogEventPropertyValue eventId))
            {
                var value = (eventId as StructureValue).Properties.FirstOrDefault(p => string.Equals(p.Name, "Name", StringComparison.InvariantCultureIgnoreCase))?.Value?.ToString()?.Replace("\"", string.Empty) ?? string.Empty;

                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("EventIdName", value, false));
            }
        }
    }
}
