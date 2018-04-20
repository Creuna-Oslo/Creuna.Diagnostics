using System;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using Creuna.ApplicationInsights.TelemetryFiltering;
// ReSharper disable RedundantUsingDirective
using Creuna.Diagnostics;
using Creuna.Diagnostics.Web;
using Creuna.Diagnostics.Web.Episerver;
// ReSharper restore RedundantUsingDirective
using EPiLog;
using EPiLog.Enrichers;
using EPiServer.Logging.Compatibility;
using EPiServer.ServiceLocation;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Operations;
using Operations.Debugging;
using Operations.Serilog;
using Operations.Trackers;
using Operations.Trackers.Profiler;
using Operations.Web.Mvc;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.ExtensionMethods;
using SerilogWeb.Classic.Enrichers;
using SerilogWeb.Classic.Mvc.Enrichers;
using SerilogWeb.Classic.WebApi.Enrichers;

namespace Creuna.Diagnostics.Web.Episerver.Internal
{
    /*  AppLogger is an entry point, don't forget to call .Startup() and .Shutdown(). 
     *  It's recommended to setup logging independently from service container. 
     *  Sample initialization in global.asax.cs:
       
            private static AppLogger AppLogger { get; private set; } 
            protected void Application_Start()
            {
                AppLogger = new AppLogger(Creuna.Diagnostics.Web.Episerver.DiagnosticsConfiguration.Current);
                AreaRegistration.RegisterAllAreas();
                GlobalConfiguration.Configure(WebApiConfig.Register);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                // ...
                AppLogger.Startup();
            }

            protected void Application_End()
            {
                AppLogger?.Shutdown();
            } 
    */

    public class AppLogger
    {
        private readonly IDiagnosticsConfiguration _configuration;
        private TelemetryClient _telemetryClient;

        public AppLogger(IDiagnosticsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual void Startup()
        {
            if (_configuration.ApplicationInsightsEnabled)
            {
                TelemetryConfiguration.Active.InstrumentationKey = _configuration.InstrumentationKey;

                if (_configuration.FilterTelemetry)
                {
                    var builder = TelemetryConfiguration.Active.TelemetryProcessorChainBuilder;
                    builder.Use(x => new FilterTelemetryProcessor(x));
                    builder.BuildAndReinitialize();
                }

                _telemetryClient = new TelemetryClient(TelemetryConfiguration.Active);
            }
            else
            {
                TelemetryConfiguration.Active.DisableTelemetry = true;
            }

            FilterTelemetryConfiguration.Getter = () => ServiceLocator.Current.GetInstance<IFilterTelemetryConfiguration>();
            OperationsSerilogDefaults.Apply();

            if (_configuration.LogActions)
            {
                GlobalFilters.Filters.Add(new OperationsActionFilter());
                GlobalConfiguration.Configuration.Services.Add(typeof(IExceptionLogger), new WebApiExceptionLogger());
            }

            var log = new LoggerConfiguration();

            if (_configuration.ApplicationInsightsEnabled)
            {
                log = log.WriteTo.ApplicationInsights(_telemetryClient, LogEventToTelemetryConverter);
            }

            if (_configuration.SeqEnabled)
            {
                log = log.WriteTo.Seq(_configuration.SeqUrl);
            }

            if (_configuration.DestructureContextData)
            {
                log.ConfigureDefaultOperationsDestructuring();
                Operations.Serilog.Factories.UseDestructuring();
            }

            log
                .MinimumLevel.Is(_configuration.LogLevel)
                // .WriteTo.RollingFile($"{logDir}EPiServer.log", shared: true)
                .WriteTo.Log4Net("episerver")
                .Enrich.WithExceptionDetails()
                .Enrich.FromLogContext()
                .Enrich.With<HttpRequestIdEnricher>()
                .Enrich.With<HttpRequestRawUrlEnricher>()
                .Enrich.With<HttpRequestUrlEnricher>()
                .Enrich.With<HttpRequestUrlReferrerEnricher>()
                .Enrich.With<HttpRequestUserAgentEnricher>()
                .Enrich.With<HttpRequestTraceIdEnricher>()
                .Enrich.With<UserNameEnricher>()
                .Enrich.With<MvcControllerNameEnricher>()
                .Enrich.With<MvcActionNameEnricher>()
                .Enrich.With<MvcRouteTemplateEnricher>()
                .Enrich.With<MvcRouteDataEnricher>()
                .Enrich.With<WebApiControllerNameEnricher>()
                .Enrich.With<WebApiActionNameEnricher>()
                .Enrich.With<WebApiRouteDataEnricher>()
                .Enrich.With<WebApiRouteTemplateEnricher>()
                .Enrich.With<EPiServerEditModeEnricher>();

            Log.Logger = log.CreateLogger();

            var now = DateTime.Now;
            EPiServer.Logging.LogManager.Instance.AddFactory(new SerilogFactory());

            var episerverLog = LogManager.GetLogger(typeof(AppLogger));

            if (_configuration.DebugLog)
            {
                Serilog.Debugging.SelfLog.Enable(LogLogMessage);

                Log.Logger.Verbose("Application started: {time}", now);
                Log.Logger.Debug("Application started: {time}", now);
                Log.Logger.Information("Application started: {time}", now);
                Log.Logger.Warning("Application started: {time}", now);
                Log.Logger.Error("Application started: {time}", now);
                Log.Logger.Fatal("Application started: {time}", now);

                episerverLog.Debug("EPISERVER Application started");
                episerverLog.Warn("EPISERVER Application started");
                episerverLog.Info("EPISERVER Application started");
                episerverLog.Error("EPISERVER Application started");
                episerverLog.Fatal("EPISERVER Application started");
            }
            else
            {
                Log.Logger.Information("Application started");
            }

            if (_configuration.DebugLog)
            {
                OperationsLog.Enable(msg => Log.ForContext<AppLogger>().Warning(msg));
            }
            else
            {
                OperationsLog.Disable();
            }

            Op.Runner = Op.Configure(x => x
                .Track.With<StatusTracker>()
                .Track.With<ProfilingTracker>()
                .Track.With<SerilogOperationTracker>()).CreateRunner();
        }

        private static void LogLogMessage(string message)
        {
            log4net.LogManager.GetLogger(typeof(AppLogger)).Error($"SERILOG: {message}");
        }

        private static ITelemetry LogEventToTelemetryConverter(LogEvent e, IFormatProvider formatProvider)
        {
            if (e.Exception != null && (e.Level == LogEventLevel.Error || e.Level == LogEventLevel.Fatal))
                return e.ToDefaultExceptionTelemetry(formatProvider,
                    includeLogLevelAsProperty: false,
                    includeRenderedMessageAsProperty: true,
                    includeMessageTemplateAsProperty: false);

            return e.ToDefaultTraceTelemetry(formatProvider,
                includeLogLevelAsProperty: true,
                includeRenderedMessageAsProperty: true,
                includeMessageTemplateAsProperty: false);
        }

        public virtual void Shutdown()
        {
            Log.Logger.Information("Application shutdown");
            _telemetryClient?.Flush();
            Log.CloseAndFlush();

            // not sure who is responsible for disposing TelemetryConfiguration singleton created by accessing TelemetryConfiguration.Active?
        }
    }
}