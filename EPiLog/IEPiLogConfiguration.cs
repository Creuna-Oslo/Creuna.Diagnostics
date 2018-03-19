using Serilog.Events;

namespace EPiLog
{
    public interface IEPiLogConfiguration
    {
        LogEventLevel GetLevel(string name);
    }
}