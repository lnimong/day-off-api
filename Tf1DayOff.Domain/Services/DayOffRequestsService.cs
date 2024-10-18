using Tf1DayOff.Domain.Entities;
using Tf1DayOff.Domain.Errors;
using Tf1DayOff.Domain.InfraInterfaces;

namespace Tf1DayOff.Domain.Services;

public class DayOffRequestsService  
{
    private readonly IDayOffRequestsRepository _store;

    public DayOffRequestsService(IDayOffRequestsRepository store) => _store = store;

    public async Task AddNewRequest(string userId, DateTime start, DateTime end, string comment, DayOffType type)
    {
        start = start.Date;
        end = end.Date;
        await EnsureNoOverlap(userId, start, end);
        var issue = new DayOffRequest(Guid.NewGuid(), userId, type, start, end);
        await _store.Save(RequestLifeCycle(DayOffRequestEvent.Creation, userId, issue, comment));
    }

    //public async Task ValidateRequest(string validatorId, Guid issueId)
    public async Task ValidateRequest(string validatorId, Guid issueId, DayOffRequestEvent action, string comment, DateTime time)
    {
        var issue = await _store.GetById(issueId);
        var newIssue = RequestLifeCycle(action, validatorId, issue, comment);
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

    private DayOffRequest RequestLifeCycle(DayOffRequestEvent dayOffRequestEvent, string userId, DayOffRequest request, string comment)
    {
        // lifecycle/state machine 
        // see figure 4 in https://learn.microsoft.com/en-us/archive/msdn-magazine/2019/may/csharp-8-0-pattern-matching-in-csharp-8-0#expressing-patterns
        return (creation: dayOffRequestEvent, request.Status) switch
        {
            (DayOffRequestEvent.Creation, DayOffRequestStatus.New) => request with { Status = DayOffRequestStatus.Pending, StatusUpdatedBy = userId, RequestComment = comment },
            (DayOffRequestEvent.Validation, DayOffRequestStatus.Pending) => request with { Status = DayOffRequestStatus.Accepted, StatusUpdatedBy = userId, StatusUpdateComment = comment },
            (DayOffRequestEvent.Refusal, DayOffRequestStatus.Pending) => request with { Status = DayOffRequestStatus.Rejected, StatusUpdatedBy = userId, StatusUpdateComment = comment }, 
            _ => throw new InvalidUserActionException($"cannot apply ${dayOffRequestEvent} on request {request.UserId} with status {request.Status}")
        };
    }
}

public enum DayOffRequestEvent
{
    Creation,
    Validation,
    Refusal
}