using System;

namespace Creuna.ApplicationInsights.TelemetryFiltering
{
    public class DefaultFilterTelemetryConfiguration : IFilterTelemetryConfiguration
    {
        public TimeSpan TraceOperationsLongerThan { get; } = TimeSpan.FromMilliseconds(1000);
    }
}