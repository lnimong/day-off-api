using System.Text.Json.Serialization;
using Tf1DayOff.App.Commands;
using Tf1DayOff.Domain.Entities;

namespace Tf1DayOff.Api.Dtos;

public class DayOffRequestDto
{
    [JsonConstructor]
    public DayOffRequestDto(DayOffType type, DateTime start, DateTime end, string comment = "")
    {
        Start = start;
        End = end;
        Comment = comment;
        Type = type;
    }

    public DayOffType Type { get; }
    public string Comment { get; }
    public DateTime Start { get; }
    public DateTime End { get; }
}


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

public class DayOffRequestDetailsResponseDto
{
    [JsonConstructor]
    public DayOffRequestDetailsResponseDto(DayOffRequestDetailsDto[] requests)
    {
        Requests = requests;
    }

    public DayOffRequestDetailsDto[] Requests { get; }
}