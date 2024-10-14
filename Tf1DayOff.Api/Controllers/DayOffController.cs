using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Tf1DayOff.Api.Constants;
using Tf1DayOff.Api.Dtos;
using Tf1DayOff.App.Commands;
using Tf1DayOff.App.Queries;
using Tf1DayOff.Domain.Entities;

namespace Tf1DayOff.Api.Controllers;

[ApiController]
[Route("day-off")]
public class DayOffController : ControllerBase
{
    private readonly IHttpContextAccessor _http;
    private readonly ISender _sender;

    public DayOffController(IHttpContextAccessor http, ISender sender)
    {
        _http = http;
        _sender = sender;
    }

    [HttpPost("new-request")]
    public async Task NewDayOffRequest(DayOffRequestDto requestDto, [FromServices] IServiceProvider serviceProvider)
    {
        var userId = _http.GetUserIdFromHeader();
        var command = new RequestDayOffCommand.Params(userId??string.Empty, requestDto.Start, requestDto.End);
        await _sender.Send(command);
    }


    [HttpPost("validate-request/{requestId}")]
    public async Task ValidateDayOffRequest(Guid requestId)
    {
        var userId = _http.GetUserIdFromHeader();
        var command = new ValidateDayOffCommand.Params(userId ?? string.Empty, requestId);
        await _sender.Send(command);
    }


    [HttpGet]
    public async Task<DayOffRequestDetailsResponseDto> GetDayOffRequests([FromQuery]string?[] status, [FromQuery]string?[] type)
    {
        var userId = _http.GetUserIdFromHeader();
        var statusValues = status.Select(StatusEnum).Where(x => x.HasValue).Select(x => x!.Value);
        var typeEValues = type.Select(TypeEnum).Where(x => x.HasValue).Select(x => x!.Value);
        var command = new GetDayOffRequestsQuery.Params(userId ?? string.Empty, 
            StatusFilter : statusValues, 
            TypeFilter : typeEValues);
        var result =await _sender.Send(command);
        
        return new DayOffRequestDetailsResponseDto(
            requests: result.Select(DayOffRequestDetailsDto.From).ToArray() 
        );
    }

    private static DayOffType? TypeEnum(string? type) =>
        type == null ? null : 
            type.ToLower() switch
            {
                "vacation" => DayOffType.Vacation,
                "sick" => DayOffType.Sick,
                _ => null
            };

private static DayOffRequestStatus? StatusEnum(string? status) =>
        status == null ? null : 
            status.ToLower() switch
            {
                "new" => DayOffRequestStatus.New,
                "pending" => DayOffRequestStatus.Pending,
                "accepted" => DayOffRequestStatus.Accepted,
                "rejected" => DayOffRequestStatus.Rejected,
                _ => null
            };
}

public static class HttpExtensions
{
    public static string? GetUserIdFromHeader(this IHttpContextAccessor http)
    {
        http.HttpContext!.Request.Headers.TryGetValue(ApiConstants.XUser, out var userId);
        return userId;
    }
}