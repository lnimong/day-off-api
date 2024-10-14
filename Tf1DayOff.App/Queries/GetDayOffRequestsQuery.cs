using FluentValidation;
using MediatR;
using Tf1DayOff.App.Helpers;
using Tf1DayOff.Domain.Entities;
using Tf1DayOff.Domain.InfraInterfaces;
using Tf1DayOff.Domain.Services;

namespace Tf1DayOff.App.Queries;

public static class GetDayOffRequestsQuery
{
    public record Params(string UserId, IEnumerable<DayOffRequestStatus>? StatusFilter,
        IEnumerable<DayOffType>? TypeFilter) : IRequest<IEnumerable<DayOffRequest>>;

    public class Validator : AbstractValidator<Params>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User Id".ShouldNotBeEmpty());
        }
    }

    public class Handler : IRequestHandler<Params, IEnumerable<DayOffRequest>>
    {
        private readonly DayOffRequestsService _requestsSvc;

        public Handler(DayOffRequestsService requestsSvc)
        {
            _requestsSvc = requestsSvc;
        }

        public Task<IEnumerable<DayOffRequest>> Handle(Params request, CancellationToken cancellationToken)
        {
            return _requestsSvc.GetRequests(withFilters:new Filters
            (
                ByUsers : new[ ] {request.UserId},
                ByStatuses : request.StatusFilter?.ToArray(),
                ByTypes : request.TypeFilter?.ToArray()
            ));
        }
    }
}