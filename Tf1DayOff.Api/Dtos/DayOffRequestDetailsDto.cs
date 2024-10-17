using System.Text.Json.Serialization;
using Tf1DayOff.Domain.Entities;

namespace Tf1DayOff.Api.Dtos;

public class DayOffRequestDetailsDto
{
    public static DayOffRequestDetailsDto From(DayOffRequest request) => new(request);

    private DayOffRequestDetailsDto(DayOffRequest request)
    {
        Id = request.Id;
        Start = request.Start;
        End = request.End;
        Status = request.Status;
        Comment = request.RequestComment;
    }


    [JsonConstructor]
    public DayOffRequestDetailsDto(Guid id, DateTime start, DateTime end, DayOffRequestStatus status, string? comment)
    {
        Id = id;
        Start = start;
        End = end;
        Status = status;
        Comment = comment;
    }

    public Guid Id { get; }
    public DateTime Start { get; }
    public DateTime End { get; }
    public DayOffRequestStatus Status { get; }
    public string? Comment { get; }
}