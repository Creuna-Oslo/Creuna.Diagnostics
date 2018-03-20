using System;
using System.Configuration;
using Creuna.ApplicationInsights.TelemetryFiltering;
using Creuna.Diagnostics.FeatureToggles;
using Serilog.Events;
using ClientSideTelemetry = Creuna.Diagnostics.Web.FeatureToggles.ClientSideTelemetry;

namespace Creuna.Diagnostics.Web.Episerver.Internal
{
    public class AppSettingsDiagnosticsConfiguration : IDiagnosticsConfiguration, IFilterTelemetryConfiguration
    {
        public AppSettingsDiagnosticsConfiguration()
        {
            LogLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel),
                ConfigurationManager.AppSettings["Diagnostics.Level"], ignoreCase: true);
            ApplicationInsightsEnabled = !string.IsNullOrEmpty(InstrumentationKey);
            ClientSideTelemetry = ApplicationInsightsEnabled && new ClientSideTelemetry().FeatureEnabled;
            SeqEnabled = !string.IsNullOrEmpty(SeqUrl);
            TraceOperationsLongerThan = TimeSpan.FromMilliseconds(int.Parse(
                ConfigurationManager.AppSettings["Diagnostics.TraceOperationsLongerThanXmsec"] ?? "1000"));
        }
        public string InstrumentationKey { get; } = ConfigurationManager.AppSettings["Diagnostics.iKey"];
        public bool ApplicationInsightsEnabled { get; }
        public LogEventLevel LogLevel { get; } 
        public bool SeqEnabled { get; }

        public string SeqUrl { get; } = ConfigurationManager.AppSettings["Diagnostics.Seq"];

        public string LogDir { get; } = ConfigurationManager.AppSettings["Diagnostics.LogDir"] ?? "~/App_Data/logs/";
        public bool LogActions { get; } = new LogActions().FeatureEnabled;
        public bool DebugLog { get; } = new DebugLog().FeatureEnabled;
        public bool FilterTelemetry { get; } = new FilterTelemetry().FeatureEnabled;
        public bool ClientSideTelemetry { get; } 

        public TimeSpan TraceOperationsLongerThan { get; }
    }
}