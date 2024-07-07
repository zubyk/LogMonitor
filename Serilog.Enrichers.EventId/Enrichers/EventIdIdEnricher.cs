using Serilog.Core;
using Serilog.Events;
using System;
using System.Linq;

namespace Serilog.Enrichers
{
    public class EventIdIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Properties.TryGetValue("EventId", out LogEventPropertyValue eventId))
            {
                var value = (eventId as StructureValue).Properties.FirstOrDefault(p => string.Equals(p.Name, "Id", StringComparison.InvariantCultureIgnoreCase))?.Value?.ToString() ?? string.Empty;

                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("EventIdId", value, false));
            }
        }
    }
}
