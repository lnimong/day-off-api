namespace Tf1DayOff.Domain.InfraInterfaces;

public interface IClock
{
    DateTime UtcNow { get; }
}