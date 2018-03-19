using System;
using System.Linq;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Serilog;

namespace Creuna.Diagnostics
{
    public static class TelemetryProcessorChainBuilderExtensions
    {
        /// <summary>
        /// This fixes https://github.com/Microsoft/ApplicationInsights-dotnet/issues/549 in v4.5.1
        /// </summary>
        public static void BuildAndReinitialize(this TelemetryProcessorChainBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            var configuration = TelemetryConfiguration.Active;
            foreach (var module in configuration.TelemetryProcessors.OfType<ITelemetryModule>())
            {
                try
                {
                    module.Initialize(configuration);
                }
                catch (Exception ex)
                {
                    Log.Logger.ForContext<AppLogger>().Error(ex, "Error initializing telemetry processor {processor}: {message}",
                        module.ToString(), ex.Message);
                }
            }
        }
    }
}