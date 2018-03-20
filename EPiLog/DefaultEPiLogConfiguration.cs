using System.Collections.Generic;
using System.Configuration;
using EPiLog.Configuration;
using EPiServer.ServiceLocation;
using Serilog.Events;

namespace EPiLog
{
    [ServiceConfiguration(typeof(IEPiLogConfiguration), Lifecycle = ServiceInstanceScope.Singleton)]
    public class DefaultEPiLogConfiguration : IEPiLogConfiguration
    {
        private readonly Dictionary<string, LogEventLevel> _levels = new Dictionary<string, LogEventLevel>();
        private LogEventLevel _defaultLevel;
        private bool _initialized = false;
        private readonly object _syncRoot = new object();

        public virtual LogEventLevel GetLevel(string name)
        {
            EnsureInitialized();
            return _levels.TryGetValue(name, out LogEventLevel level) ? level : _defaultLevel;
        }

        protected virtual void Initialize()
        {
            var section = ConfigurationManager.GetSection(EPiLogConfigurationSection.SectionName) as EPiLogConfigurationSection;
            if (section == null)
            {
                throw new ConfigurationErrorsException($"Missed configuration section <{EPiLogConfigurationSection.SectionName} />");
            }

            _defaultLevel = section.DefaultLevel;
            foreach (LogConfigurationElement logLevel in section.LogLevels)
            {
                _levels[logLevel.Log] = logLevel.Level.GetValueOrDefault(_defaultLevel);
            }
        }

        protected void EnsureInitialized()
        {
            if (!_initialized)
            {
                lock (_syncRoot)
                {
                    if (!_initialized)
                    {
                        Initialize();
                        _initialized = true;
                    }
                }
            }
        }
    }
}