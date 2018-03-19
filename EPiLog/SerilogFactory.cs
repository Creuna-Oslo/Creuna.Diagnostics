using EPiServer.Logging;

namespace EPiLog
{
    public class SerilogFactory : ILoggerFactory
    {
        public virtual ILogger Create(string name)
        {
            return new SerilogLogger(name);
        }
    }
}