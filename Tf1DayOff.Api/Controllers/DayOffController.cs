using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tf1DayOff.Api.Constants;
using Tf1DayOff.Api.Dtos;
using Tf1DayOff.App.Commands;
using Tf1DayOff.App.Queries;
using Tf1DayOff.Domain.Entities;
using Tf1DayOff.Domain.Services;

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


    /// <summary>
    /// creates a new day off request
    /// the user who creates the request is passed as a header (X-User)
    /// </summary>
    /// <param name="requestDto">
    /// the request details
    /// - type : possible Values are "Sick" or "Vacation"
    /// - comment :  a string that add complementary details to the request
    /// - start : the start date of the request
    /// - end : the end date of the request
    /// </param>
    /// <returns></returns>
    [HttpPost("new-request")]
    public async Task NewDayOffRequest(DayOffRequestDto requestDto)
    {
        var userId = _http.GetUserIdFromHeader();
        var command = new RequestDayOffCommand.Params(userId??string.Empty, requestDto.Start, requestDto.End, requestDto.Comment, requestDto.Type);
        await _sender.Send(command);
    }

    /// <summary>
    /// validate a new day off request
    /// the user who validate the request is passed as a header (X-User)
    /// </summary>
    /// <param name="requestId">the id of the request to be validated</param>
    /// <param name="request">
    /// the validation details
    /// - comment :  a string that add complementary details to the request validation (or refusal)
    /// - action : possible values are "Validate" or "Reject"
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [HttpPost("validate-request/{requestId}")]
    public async Task ValidateDayOffRequest(Guid requestId, DayOffValidationRequestDto request)
    {
        var userId = _http.GetUserIdFromHeader();
        var action = request.Action switch
        {
            DayOffValidationAction.Validate => DayOffRequestEvent.Validation,
            DayOffValidationAction.Reject => DayOffRequestEvent.Refusal,
            _ => throw new ArgumentOutOfRangeException(nameof(request.Action))
        };
        var command = new ValidateDayOffCommand.Params(userId ?? string.Empty, requestId, action, request.Comment);
        await _sender.Send(command);
    }

    /// <summary>
    /// retrieve the list of day off requests for the current user
    /// </summary>
    /// <param name="status">the status filter (non mandatory)</param>
    /// <param name="type">the type filter (non mandatory)</param>
    /// <returns></returns>
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