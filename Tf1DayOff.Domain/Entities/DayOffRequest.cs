using System.Text.Json.Serialization;
using Tf1DayOff.Domain.Services;

namespace Tf1DayOff.Domain.Entities;

public enum DayOffRequestStatus
{
    New,
    Pending,
    Accepted,
    Rejected
}


public enum DayOffType
{
    Sick, Vacation
}

public record DayOffRequest(Guid Id, string UserId, DayOffType Type, DateTime Start, DateTime End,
    DayOffRequestStatus Status = DayOffRequestStatus.New,
    string? StatusUpdatedBy = null,
    string? RequestComment = null,
    string StatusUpdateComment = "");