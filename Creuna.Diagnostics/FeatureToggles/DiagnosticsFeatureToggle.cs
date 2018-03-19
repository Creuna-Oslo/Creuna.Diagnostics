using System.Configuration;
using FeatureToggle.Core;

namespace Creuna.Diagnostics.FeatureToggles
{
    public abstract class DiagnosticsFeatureToggle : IFeatureToggle
    {
        public static string KeyPrefix { get; set; } = "Diagnostics.";

        protected virtual string AppSettingsKeyFor(IFeatureToggle toggle)
        {
            return $"{KeyPrefix}{toggle.GetType().Name}";
        }

        protected abstract bool Default { get; }

        public virtual bool FeatureEnabled
        {
            get
            {
                bool configValue;
                var result = bool.TryParse(ConfigurationManager.AppSettings[AppSettingsKeyFor(this)], out configValue)
                    ? configValue
                    : Default;
                return result;
            }
        }
    }
}
