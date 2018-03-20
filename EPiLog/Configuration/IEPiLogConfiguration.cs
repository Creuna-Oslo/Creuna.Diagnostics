using Serilog.Events;

namespace EPiLog.Configuration
{
    public interface IEPiLogConfiguration
    {
        LogEventLevel GetLevel(string name);
    }
}