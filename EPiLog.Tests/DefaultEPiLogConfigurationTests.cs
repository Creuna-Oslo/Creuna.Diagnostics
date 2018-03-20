using FluentAssertions;
using NUnit.Framework;
using Serilog.Events;

namespace EPiLog
{
    public class DefaultEPiLogConfigurationTests
    {
        [Test]
        public void Acceptance()
        {
            var configuration = EPiLogConfiguration.Current;
            var expectedDefaultLevel = LogEventLevel.Warning;
            configuration.GetLevel("default").Should().Be(expectedDefaultLevel);
            configuration.GetLevel("verbose").Should().Be(LogEventLevel.Verbose);
            configuration.GetLevel("debug").Should().Be(LogEventLevel.Debug);
            configuration.GetLevel("info").Should().Be(LogEventLevel.Information);
            configuration.GetLevel("warn").Should().Be(LogEventLevel.Warning);
            configuration.GetLevel("error").Should().Be(LogEventLevel.Error);
            configuration.GetLevel("fatal").Should().Be(LogEventLevel.Fatal);
            configuration.GetLevel("not-in-config").Should().Be(expectedDefaultLevel);
        }
    }
}
