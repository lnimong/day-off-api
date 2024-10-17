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