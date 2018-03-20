using System.Configuration;
using Serilog.Events;

namespace EPiLog.Configuration
{
    public class EPiLogConfigurationSection : ConfigurationSection
    {
        public const string SectionName = "epilog";
        private const string LogLevelsElementName = "logLevels";
        private const string DefaultLevelAttribute = "defaultLevel";

        [ConfigurationProperty(DefaultLevelAttribute, DefaultValue = LogEventLevel.Error)]
        public LogEventLevel DefaultLevel
        {
            get => (LogEventLevel)this[DefaultLevelAttribute];
            set => this[DefaultLevelAttribute] = value;
        }

        [ConfigurationProperty(LogLevelsElementName)]
        public LogsConfigurationCollection LogLevels
        {
            get => (LogsConfigurationCollection) base[LogLevelsElementName];
            set => base[LogLevelsElementName] = value;
        }
    }
}