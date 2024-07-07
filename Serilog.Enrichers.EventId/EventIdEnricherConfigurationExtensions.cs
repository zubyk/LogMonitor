using Serilog.Configuration;
using Serilog.Enrichers;
using System;

namespace Serilog
{
    public static class EventIdEnricherConfigurationExtensions
    {
        public static LoggerConfiguration WithEventIdId(
           this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
            return enrichmentConfiguration.With<EventIdIdEnricher>();
        }

        public static LoggerConfiguration WithEventIdName(
           this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));          
            return enrichmentConfiguration.With<EventIdNameEnricher>();
        }
    }
}