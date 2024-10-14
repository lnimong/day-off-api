using Tf1DayOff.Domain.Entities;
using Tf1DayOff.Domain.Errors;
using Tf1DayOff.Domain.InfraInterfaces;

namespace Tf1DayOff.Domain.Services;

public class DayOffRequestsService  
{
    private readonly IDayOffRequestsRepository _store;

    public DayOffRequestsService(IDayOffRequestsRepository store) => _store = store;

    public async Task AddNewRequest(string userId, DateTime start, DateTime end)
    {
        start = start.Date;
        end = end.Date;
        await EnsureNoOverlap(userId, start, end);
        var issue = new DayOffRequest(Guid.NewGuid(), userId, DayOffType.Vacation, start, end);
        await _store.Save(RequestLifeCycle(Event.Creation, userId, issue));
    }

    public async Task ValidateRequest(string validatorId, Guid issueId)
    {
        var issue = await _store.GetById(issueId);
        var newIssue = RequestLifeCycle(Event.Validation, validatorId, issue);
        await _store.Save(newIssue);
    }

    public Task<IEnumerable<DayOffRequest>> GetRequests(Filters withFilters)
    {
        return _store.GetWithFilters(withFilters);
    }

    private async Task EnsureNoOverlap(string userId, DateTime start, DateTime end)
    {
        var overlaps = await _store.FindOverlaps(userId, start, end);
        if (overlaps.Any()) 
            throw new InvalidUserActionException("Day off request overlaps with existing day off tickets");
    }

    private enum Event
    {
        Creation,
        Validation,
        Refusal
    }
    private DayOffRequest RequestLifeCycle(Event @event, string userId, DayOffRequest request)
    {
        // lifecycle/state machine 
        // see figure 4 in https://learn.microsoft.com/en-us/archive/msdn-magazine/2019/may/csharp-8-0-pattern-matching-in-csharp-8-0#expressing-patterns
        return (creation: @event, request.Status) switch
        {
            (Event.Creation, DayOffRequestStatus.New) => request with { Status = DayOffRequestStatus.Pending, StatusUpdatedBy = userId },
            (Event.Validation, DayOffRequestStatus.Pending) => request with { Status = DayOffRequestStatus.Accepted, StatusUpdatedBy = userId },
            (Event.Refusal, DayOffRequestStatus.Pending) => request with { Status = DayOffRequestStatus.Rejected, StatusUpdatedBy = userId }, // here just to get a better idea of what and entity state machine could look like
            _ => throw new InvalidUserActionException($"cannot apply ${@event} on request {request.UserId} with status {request.Status}")
        };
    }
}