using System;
using System.Collections.Generic;
using System.Configuration;
using Creuna.Diagnostics.FeatureToggles;
using Serilog.Events;
using ClientSideTelemetry = Creuna.Diagnostics.Web.FeatureToggles.ClientSideTelemetry;

namespace Creuna.Diagnostics.Web.Episerver
{
    public class AppSettingsDiagnosticsConfiguration : IDiagnosticsConfiguration
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

        public Dictionary<string, LogEventLevel> EnabledLevels = GetDefaultLevels();

        private static Dictionary<string, LogEventLevel> GetDefaultLevels()
        {
            return new Dictionary<string, LogEventLevel>
            {
                { "EPiServer.Core.OptimisticCache", LogEventLevel.Error },
                { "EPiServer.Core.ContentProvider", LogEventLevel.Error },
                { "EPiServer.Data.Dynamic.Providers.DbDataStoreProvider", LogEventLevel.Error },
                { "EPiServer.Data.Providers.SqlDatabaseHandler", LogEventLevel.Error },
                { "EPiServer.Data.Providers.ConnectionContext", LogEventLevel.Error },
                { "EPiServer.Framework.Initialization.InitializationEngine", LogEventLevel.Error },
            };
        }

        public virtual LogEventLevel GetLevel(string name)
        {
            var result = EnabledLevels.ContainsKey(name)
                ? EnabledLevels[name]
                : LogLevel;

            return result;
        }
    }
}