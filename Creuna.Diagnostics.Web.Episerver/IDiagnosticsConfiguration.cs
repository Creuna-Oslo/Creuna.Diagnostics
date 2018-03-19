using System;
using Serilog.Events;

namespace Creuna.Diagnostics.Web.Episerver
{
    public interface IDiagnosticsConfiguration
    {
        string InstrumentationKey { get; }
        bool ApplicationInsightsEnabled { get; }
        LogEventLevel LogLevel { get; }
        bool SeqEnabled { get; }
        string SeqUrl { get; }
        string LogDir { get; }
        bool LogActions { get; }
        bool DebugLog { get; }
        bool FilterTelemetry { get; }
        bool ClientSideTelemetry { get; }
        TimeSpan TraceOperationsLongerThan { get; }
        LogEventLevel GetLevel(string name);
    }
}