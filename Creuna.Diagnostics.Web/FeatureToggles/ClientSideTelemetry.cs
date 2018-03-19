using Creuna.Diagnostics.FeatureToggles;

namespace Creuna.Diagnostics.Web.FeatureToggles
{
    public class ClientSideTelemetry : DiagnosticsFeatureToggle
    {
        protected override bool Default => false;
    }
}