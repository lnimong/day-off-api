using Tf1DayOff.Domain.Entities;

namespace Tf1DayOff.Domain.InfraInterfaces;

public interface IDayOffRequestsRepository
{
    Task<DayOffRequest> GetById(Guid id);
    Task Save(DayOffRequest request);
    Task<IEnumerable<DayOffRequest>> FindOverlaps(string userId, DateTime start, DateTime end);
    Task<IEnumerable<DayOffRequest>> GetWithFilters(Filters filters);
}

public record Filters(string[]? ByUsers, DayOffRequestStatus[]? ByStatuses,
    DayOffType[]? ByTypes);