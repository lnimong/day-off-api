using Tf1DayOff.Domain.Entities;
using Tf1DayOff.Domain.InfraInterfaces;
using Tf1DayOff.Infra.Helpers;

namespace Tf1DayOff.Infra.Repositories;

public abstract class TemplateDayOffRequestsRepository : IDayOffRequestsRepository
{
    public async Task<DayOffRequest> GetById(Guid id) => (await Storage())[id];

    public async Task Save(DayOffRequest request)
    {
        var d = (await Storage()).AddOrUpdate(request.Id, request, (_, _) => request);
        await Save(d);
    }

    public async Task<IEnumerable<DayOffRequest>> FindOverlaps(string userId, DateTime start, DateTime end) =>
        (await Storage()).Values.Where(
            x =>
                x.UserId == userId &&
                (x.Start >= start || x.End >= start) &&
                (x.End <= end || x.Start <= end)
        );

    public async Task<IEnumerable<DayOffRequest>> GetWithFilters(Filters filters) =>
        (await Storage()).Values.Where(
            x =>
                x.UserId.Matches(filters.ByUsers) &&
                x.Type.Matches(filters.ByTypes) &&
                x.Status.Matches(filters.ByStatuses)
        );

    protected abstract Task<IDictionary<Guid, DayOffRequest>> Storage();
    protected abstract Task Save(IDictionary<Guid, DayOffRequest> request);

}