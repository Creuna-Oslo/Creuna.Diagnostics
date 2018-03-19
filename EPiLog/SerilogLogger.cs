using System;
using EPiServer.Logging;
using Serilog.Events;
using ILogger = Serilog.ILogger;
using LoggerExtensions = Serilog.LoggerExtensions;

namespace EPiLog
{
    /// <summary>
    /// Class SeriLogger.
    /// </summary>
    public class SerilogLogger : EPiServer.Logging.ILogger
    {
        /// <summary>
        ///     The logger
        /// </summary>
        protected virtual ILogger Logger { get; }
        public virtual string LoggerName { get; }
        protected LogEventLevel FilterLevel { get; }
        protected IEPiLogConfiguration Configuration => EPiLogConfiguration.Current;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogLogger"/> class.
        /// </summary>
        /// <param name="name">Name of the logger</param>
        public SerilogLogger(string name)
        {
            Logger = Serilog.Log.Logger;
            LoggerName = name;
            FilterLevel = Configuration.GetLevel(name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogLogger"/> class.
        /// </summary>
        public SerilogLogger() : this(null)
        {
        }

        /// <summary>
        /// Determines whether logging at the specified level is enabled.
        /// </summary>
        /// <param name="level">The level to check.</param>
        /// <returns>
        /// <c>true</c> if logging on the provided level is enabled; otherwise <c>false</c>
        /// </returns>
        public bool IsEnabled(Level level)
        {
            try
            {
                return this.IsEnabled(MapLevel(level));
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        /// <summary>
        ///     Determines whether the specified level is enabled.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns><c>true</c> if the specified level is enabled; otherwise, <c>false</c>.</returns>
        public bool IsEnabled(LogEventLevel level)
        {
            return level >= FilterLevel && Logger.IsEnabled(level);
        }

        /// <summary>
        /// Logs the provided <paramref name="state"/> with the specified level.
        /// </summary>
        /// <typeparam name="TState">The type of the state object.</typeparam><typeparam name="TException">The type of the exception.</typeparam><param name="level">The criticality level of the log message.</param><param name="state">The state that should be logged.</param><param name="exception">The exception that should be logged.</param><param name="messageFormatter">The message formatter used to write the state to the log provider.</param><param name="boundaryType">The type at the boundary of the logging framework facing the code using the logging.</param>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        /// <exception cref="ArgumentOutOfRangeException">level</exception>
        public void Log<TState, TException>(
            Level level,
            TState state,
            TException exception,
            Func<TState, TException, string> messageFormatter,
            Type boundaryType) where TException : Exception
        {
            if (messageFormatter == null)
            {
                return;
            }

            LogEventLevel mappedLevel = MapLevel(level);

            if (!this.IsEnabled(mappedLevel))
            {
                return;
            }

            var log = boundaryType != null && boundaryType != typeof(LoggerExtensions)
                ? Logger.ForContext(boundaryType)
                : Logger;
            //if ((boundaryType != null) && ())
            //{
            //    this.Logger.ForContext(boundaryType);

            //    // global::Serilog.Log.ForContext(boundaryType);
            //}

            if (LoggerName != null)
            {
                log.Write(mappedLevel, exception, $"{{logger}} {messageFormatter(state, exception)}", LoggerName);
            }
            else
            {
                log.Write(mappedLevel, exception, messageFormatter(state, exception));
            }
            // global::Serilog.Log.Write(mappedLevel, exception, messageFormatter(state, exception));
        }

        /// <summary>
        /// Closes the logger instance and flushes it.
        /// </summary>
        public void CloseAndFlush()
        {
            // The one called .CreateLogger() controls its lifetime. Dispose should be done there. 
        }

        /// <summary>
        ///     Maps the EPiServer level to the <see cref="LogEventLevel"/> level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The mapped <see cref="LogEventLevel"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The given <paramref name="level"/> can't be mapped.</exception>
        private static LogEventLevel MapLevel(Level level)
        {
            switch (level)
            {
                case Level.Trace:
                    return LogEventLevel.Verbose;
                case Level.Debug:
                    return LogEventLevel.Debug;
                case Level.Information:
                    return LogEventLevel.Information;
                case Level.Warning:
                    return LogEventLevel.Warning;
                case Level.Error:
                    return LogEventLevel.Error;
                case Level.Critical:
                    return LogEventLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level));
            }
        }
    }
}