using System.Web;
using System.Web.Http.ExceptionHandling;
using Operations.Web;

namespace Creuna.Diagnostics.Web
{
    /// <summary>
    /// This one is to write errors to Application Insights, 
    /// because SerilogWeb.Classic.WebApi just sets exception to context and it's written later 
    /// by SerilogWeb.Classic.ApplicationLifecycleModule without Exception set in the log event
    /// </summary>
    public class WebApiExceptionLogger : ExceptionLogger
    {
        private HttpContextBase HttpContext => HttpContextAccessor.Current;

        public override void Log(ExceptionLoggerContext context)
        {
            Serilog.Log.Logger.ForContext<WebApiExceptionLogger>().Error(context.Exception, "HTTP {method} {url} - {errorMessage}",
                context.Request.Method.ToString(), context.Request.RequestUri, context.Exception.Message);
        }
    }
}