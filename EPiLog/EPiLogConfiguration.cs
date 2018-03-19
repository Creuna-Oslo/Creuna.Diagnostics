using System;

namespace EPiLog
{
    public class EPiLogConfiguration
    {
        private static readonly IEPiLogConfiguration Default = new DefaultEPiLogConfiguration();
        public static Func<IEPiLogConfiguration> Getter { get; set; } = () => Default;
        public static IEPiLogConfiguration Current => Getter();
    }
}