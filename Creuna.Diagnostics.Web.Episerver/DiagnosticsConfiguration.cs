using System;

namespace Creuna.Diagnostics.Web.Episerver
{
    public static class DiagnosticsConfiguration
    {
        private static readonly IDiagnosticsConfiguration DefaultConfiguration = new AppSettingsDiagnosticsConfiguration();
        public static Func<IDiagnosticsConfiguration> Getter { get; set; } = () => DefaultConfiguration;
        public static IDiagnosticsConfiguration Current => Getter();
    }
}