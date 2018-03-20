using System.Configuration;

namespace EPiLog.Configuration
{
    [ConfigurationCollection(typeof(LogConfigurationElement))]
    public class LogsConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new LogConfigurationElement();

        protected override object GetElementKey(ConfigurationElement element) =>
            ((LogConfigurationElement) element).Log;
    }
}