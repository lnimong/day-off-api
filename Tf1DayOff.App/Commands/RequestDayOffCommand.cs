using FluentValidation;
using MediatR;
using Tf1DayOff.App.Helpers;
using Tf1DayOff.Domain.Entities;
using Tf1DayOff.Domain.InfraInterfaces;
using Tf1DayOff.Domain.Services;

namespace Tf1DayOff.App.Commands;


public static class RequestDayOffCommand
{
    public record Params(string UserId, DateTime Start, DateTime End, string Comment, DayOffType Type) : IRequest;

    public class Validator : AbstractValidator<Params>
    {
        public Validator(IClock clock)
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User Id".ShouldNotBeEmpty());
            RuleFor(x => x.Start).NotEmpty().WithMessage("Start date".ShouldNotBeEmpty());
            RuleFor(x => x.End).NotEmpty().WithMessage("end date".ShouldNotBeEmpty());
            RuleFor(x => x.Start.Date).GreaterThan(clock.UtcNow.Date)
                .WithMessage("Start date".ShouldBeBefore("today"));
            RuleFor(x => x.Start.Date).LessThanOrEqualTo(x => x.End.Date)
                .WithMessage("Start date".ShouldBeBefore("End date"));
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
            await _requestsSvc.AddNewRequest(request.UserId, request.Start, request.End, request.Comment, request.Type);
        }
    }
}
