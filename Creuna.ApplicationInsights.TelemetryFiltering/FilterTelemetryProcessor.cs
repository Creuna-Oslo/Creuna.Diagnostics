using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace Creuna.ApplicationInsights.TelemetryFiltering
{
    public class FilterTelemetryProcessor : ITelemetryProcessor
    {
        public FilterTelemetryProcessor(ITelemetryProcessor next)
        {
            Next = next;
        }

        protected ITelemetryProcessor Next { get; }
        protected IFilterTelemetryConfiguration Configuration => FilterTelemetryConfiguration.Current;
        public virtual void Process(ITelemetry item)
        {
            var operation = item as OperationTelemetry;
            if (operation != null)
            {
                if (operation.Duration < Configuration.TraceOperationsLongerThan)
                {
                    var request = item as RequestTelemetry;

                    if (request != null)
                    {
                        if (request.Success.GetValueOrDefault(false) 
                            || (request.ResponseCode?.Equals("404", StringComparison.OrdinalIgnoreCase) ?? false))
                        {
                            // just skip the telemetry for success or 404 requests
                            return;
                        }
                    }
                    else
                    {
                        if (operation.Success.GetValueOrDefault(false))
                        {
                            return;
                        }
                    }
                }
            }

            Next.Process(item);
        }
    }
}