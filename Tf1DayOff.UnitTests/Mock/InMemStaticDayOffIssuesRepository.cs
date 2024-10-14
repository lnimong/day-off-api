using System.Text.Json;
using Tf1DayOff.Domain.Entities;
using Tf1DayOff.Domain.InfraInterfaces;
using Tf1DayOff.Infra.Helpers;
using Tf1DayOff.Infra.Repositories;

namespace Tf1DayOff.UnitTests.Mock;


public class InMemStaticDayOffRequestsRepository : TemplateDayOffRequestsRepository
{ 
    public Dictionary<Guid, DayOffRequest> Data { get; } = new();

    protected override async Task<IDictionary<Guid, DayOffRequest>> Storage()
    {
        return Data;
    }

    protected override async Task Save(IDictionary<Guid, DayOffRequest> request)
    {
    }
}
