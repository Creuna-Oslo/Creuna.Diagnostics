using System;
using System.Web;
using EPiServer.Editor;
using Serilog.Core;
using Serilog.Events;

namespace EPiLog.Enrichers
{
    public class EPiServerEditModeEnricher : ILogEventEnricher
    {
        public const string PropertyName = "IsEditMode";

        public virtual void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (propertyFactory == null) throw new ArgumentNullException(nameof(propertyFactory));
            if (HttpContext.Current == null)
                return;

            var property = propertyFactory.CreateProperty(PropertyName, PageEditing.PageIsInEditMode);
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}