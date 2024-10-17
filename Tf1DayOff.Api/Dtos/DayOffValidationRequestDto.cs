namespace Tf1DayOff.Api.Dtos;

public class DayOffValidationRequestDto
{
    public string Comment { get; set; } = string.Empty;
    public DayOffValidationAction Action { get; set; }
}