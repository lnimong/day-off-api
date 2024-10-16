using FluentValidation;
using MediatR;
using Tf1DayOff.App.Helpers;
using Tf1DayOff.Domain.InfraInterfaces;
using Tf1DayOff.Domain.Services;

namespace Tf1DayOff.App.Commands;

public static class ValidateDayOffCommand
{
    public record Params(string UserId, Guid IssueId, DayOffRequestEvent Action, string Comment) : IRequest;

    public class Validator : AbstractValidator<Params>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User Id".ShouldNotBeEmpty());
        }
    }

    public class Handler : IRequestHandler<Params>
    {
        private readonly DayOffRequestsService _requestsSvc;
        private readonly IClock _clock;

        public Handler(DayOffRequestsService requestsSvc, IClock clock)
        {
            _requestsSvc = requestsSvc;
            _clock = clock;
        }

        public async Task Handle(Params request, CancellationToken cancellationToken)
        {
            await _requestsSvc.ValidateRequest(request.UserId, request.IssueId, request.Action, request.Comment, _clock.UtcNow);
        }
    }
}

