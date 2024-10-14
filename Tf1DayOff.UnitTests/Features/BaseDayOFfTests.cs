using NSubstitute;
using Tf1DayOff.Domain.Entities;
using Tf1DayOff.Domain.InfraInterfaces;
using Tf1DayOff.Infra.Helpers;
using Tf1DayOff.UnitTests.Mock;
using Tf1DayOff.UnitTests.TestInitTools;

namespace Tf1DayOff.UnitTests.Features;

public abstract class BaseDayOFfTests
{
    protected HttpClient Client = null!;
    protected IClock FakeClock = null!;
    protected InMemStaticDayOffRequestsRepository Repository = null!;



    [SetUp]
    public void Setup()
    {
        FakeClock = Substitute.For<IClock>();
        Repository = new InMemStaticDayOffRequestsRepository();

        Client = HttpClientFactory.CreateHttpClientFor<Tf1DayOFf.Api.Program>(
            registerMock: x =>
            {
                x.MockDependency(FakeClock);
                x.MockDependency<IDayOffRequestsRepository>(Repository);
            });
    }

    [TearDown]
    public void TearDown()
    {
        Client.Dispose();
    }
    protected void GivenCurrentDayOffRequestAre(params DayOffRequest[] requests)
    {
        foreach (var request in requests)
        {
            Repository.Data.AddOrUpdate(request.Id, request, (_, _) => request);
        }
    }

    protected void GivenTodayIs(DateTime date) => FakeClock.DateIs(date);
    protected DateTime Oct(int day) => new(2024, 10, day);
}