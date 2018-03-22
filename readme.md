Creuna.Diagnostics.Web.Episerver 
================================

## Features
- adds a structured logging dependencies using Serilog 
- provides a default configuration of the logging sub-system to write to (see AppLogger.Startup() for details):
-- Application Insights
-- log4net (using standard EPiServerLog.config with additional filters configured in a newly added <epilog/> section)
-- seq (it's a great tool to use at least locally)
- registers episerver ILogger/ILoggerFactory and this way enriches everything logged by EPiServer with serilog context (EPiLog package does this job)

## To start using 

1. Rename AppLogger.Sample.cs to AppLogger.cs (a new AppLogger.Sample.cs will show what's new when package is updated)
2. create a new singleton AppLogger and call Startup() on app start and Shutdown() on application shutdown. It could be done in the global.asax.cs
```
public class MyEPiServerApp : EPiServer.Global
{           
    private AppLogger AppLogger { get; } = new AppLogger(Creuna.Diagnostics.Web.Episerver.DiagnosticsConfiguration.Current);
        
	protected void Application_Start()
    {
        AreaRegistration.RegisterAllAreas();
        GlobalConfiguration.Configure(WebApiConfig.Register);
        RouteConfig.RegisterRoutes(RouteTable.Routes);
            
		// ...
        AppLogger.Startup();
    }

    protected void Application_End()
    {
        AppLogger.Shutdown();
    } 
}
```
3. Setup 3rd-parties
     - setup seq if you want to use it by adding its url to the <add key="Diagnostics.Seq" value="" /> app setting
	 - setup app insights account by adding its instrumentation key into the <add key="Diagnostics.iKey" value="" /> app setting
4. When using with App Insights 
	 - telemetry is filtered by default, set <add key="Diagnostics.FilterTelemetry" value="false" /> for production
	 - Creuna.Diagnostics.FeatureToggles.ClientSideTelemetry feature toggle should be handled manually. 
	   Add something like the following just before the end of your </head> tag in the shared layout (Views/Shared/_Layout.cshtml or similar)
```
@using Creuna.Diagnostics.Web.Episerver
@*...*@
<head>
	@*...*@

	@if (DiagnosticsConfiguration.Current.ClientSideTelemetry)
	{
		<script type='text/javascript'>
			var appInsights = window.appInsights || function (config) {
				function r(config) { t[config] = function () { var i = arguments; t.queue.push(function () { t[config].apply(t, i) }) } }
				var t = { config: config }, u = document, e = window, o = 'script', s = u.createElement(o), i, f; for (s.src = config.url || '//az416426.vo.msecnd.net/scripts/a/ai.0.js', u.getElementsByTagName(o)[0].parentNode.appendChild(s), t.cookie = u.cookie, t.queue = [], i = ['Event', 'Exception', 'Metric', 'PageView', 'Trace', 'Ajax']; i.length;)r('track' + i.pop()); return r('setAuthenticatedUserContext'), r('clearAuthenticatedUserContext'), config.disableExceptionTracking || (i = 'onerror', r('_' + i), f = e[i], e[i] = function (config, r, u, e, o) { var s = f && f(config, r, u, e, o); return s !== !0 && t['_' + i](config, r, u, e, o), s }), t
			}({
				instrumentationKey: '@DiagnosticsConfiguration.Current.InstrumentationKey'
			});

			window.appInsights = appInsights;
			appInsights.trackPageView();
		</script>
	}
</head>
```

## Release Notes
  - v0.9.0 - Initial release 