using NSubstitute;
using Tf1DayOff.Domain.InfraInterfaces;

namespace Tf1DayOff.UnitTests.Features;
public static class MockExtensions
{
    public static void DateIs(this IClock fakeClock, DateTime date) => fakeClock.UtcNow.Returns(date);
    public static Guid AsGuid(this int value) => new($"{value}".PadLeft(32, '0'));
}

