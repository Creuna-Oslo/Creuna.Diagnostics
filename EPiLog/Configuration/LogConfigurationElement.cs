using System.Configuration;
using Serilog.Events;

namespace EPiLog.Configuration
{
    public class LogConfigurationElement : ConfigurationElement
    {
        private const string LevelAttribute = "level";
        private const string LogAttribute = "log";

        [ConfigurationProperty(LogAttribute, IsRequired = true, IsKey = true)]
        public string Log
        {
            get => (string) this[LogAttribute];
            set => this[LogAttribute] = value;
        }

        [ConfigurationProperty(LevelAttribute, DefaultValue = null)]
        public LogEventLevel? Level
        {
            get => (LogEventLevel?) this[LevelAttribute];
            set => this[LevelAttribute] = value;
        }
    }
}