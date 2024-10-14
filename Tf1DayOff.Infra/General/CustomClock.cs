using Tf1DayOff.Domain.InfraInterfaces;

namespace Tf1DayOff.Infra.General;

public class CustomClock : IClock // Infra
{
    public DateTime UtcNow => DateTime.UtcNow;
}
