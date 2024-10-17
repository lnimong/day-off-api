using System.Text.Json.Serialization;

namespace Tf1DayOff.Api.Dtos;

public class DayOffRequestDetailsResponseDto
{
    [JsonConstructor]
    public DayOffRequestDetailsResponseDto(DayOffRequestDetailsDto[] requests)
    {
        Requests = requests;
    }

    public DayOffRequestDetailsDto[] Requests { get; }
}