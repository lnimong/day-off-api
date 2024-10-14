using FluentValidation;
using MediatR;
using Tf1DayOff.App.Helpers;
using Tf1DayOff.Domain.InfraInterfaces;
using Tf1DayOff.Domain.Services;

namespace Tf1DayOff.App.Commands;

public static class ValidateDayOffCommand
{
    public record Params(string UserId, Guid IssueId) : IRequest;

    public class Validator : AbstractValidator<Params>
    {
        public Validator(IClock clock)
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User Id".ShouldNotBeEmpty());
        }
    }

    public class Handler : IRequestHandler<Params>
    {
        private readonly DayOffRequestsService _requestsSvc;

        public Handler(DayOffRequestsService requestsSvc)
        {
            _requestsSvc = requestsSvc;
        }

        public async Task Handle(Params request, CancellationToken cancellationToken)
        {
            await _requestsSvc.ValidateRequest(request.UserId, request.IssueId);
        }
    }
}