using System;

namespace Creuna.ApplicationInsights.TelemetryFiltering
{
    public class FilterTelemetryConfiguration
    {
        private static readonly IFilterTelemetryConfiguration Default = new DefaultFilterTelemetryConfiguration();
        public static Func<IFilterTelemetryConfiguration> Getter { get; set; } = () => Default;
        public static IFilterTelemetryConfiguration Current => Getter();
    }
}