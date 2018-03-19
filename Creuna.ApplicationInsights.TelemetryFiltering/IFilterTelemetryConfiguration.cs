using System;

namespace Creuna.ApplicationInsights.TelemetryFiltering
{
    public interface IFilterTelemetryConfiguration
    {
        TimeSpan TraceOperationsLongerThan { get; }
    }
}