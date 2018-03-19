using System.Collections.Generic;
using Serilog.Events;

namespace EPiLog
{
    public class DefaultEPiLogConfiguration : IEPiLogConfiguration
    {
        public Dictionary<string, LogEventLevel> EnabledLevels = GetDefaultLevels();
        public virtual LogEventLevel LogLevel => LogEventLevel.Information;
        public virtual LogEventLevel GetLevel(string name)
        {
            var result = EnabledLevels.ContainsKey(name)
                ? EnabledLevels[name]
                : LogLevel;

            return result;
        }

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
    }
}