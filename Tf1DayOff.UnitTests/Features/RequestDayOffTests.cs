using FluentAssertions;
using Tf1DayOff.Api.Dtos;
using Tf1DayOff.Domain.Entities;
using Tf1DayOff.UnitTests.TestInitTools;

namespace Tf1DayOff.UnitTests.Features;

public class RequestDayOffTests : BaseDayOFfTests
{
    [Test]
    public async Task The_Should_Be_Able_To_See_Only_His_Requests()
    {
        // Arrange
        var theUser = "u1";
        var theOtherUser = "u2";
        GivenTodayIs(Oct(10));
        GivenCurrentDayOffRequestAre(
            new DayOffRequest(1.AsGuid(), theUser, DayOffType.Vacation, Oct(2), Oct(6), DayOffRequestStatus.Pending),
            new DayOffRequest(2.AsGuid(), theUser, DayOffType.Vacation, Oct(10), Oct(16), DayOffRequestStatus.Pending),
            new DayOffRequest(3.AsGuid(), theUser, DayOffType.Vacation, Oct(20), Oct(26), DayOffRequestStatus.Pending),
            new DayOffRequest(4.AsGuid(), theOtherUser, DayOffType.Vacation, Oct(20), Oct(26), DayOffRequestStatus.Pending),
            new DayOffRequest(5.AsGuid(), theOtherUser, DayOffType.Vacation, Oct(27), Oct(30), DayOffRequestStatus.Pending),
            new DayOffRequest(6.AsGuid(), theUser, DayOffType.Vacation, Oct(1), Oct(7), DayOffRequestStatus.Pending)
        );

        // Act
        var (response, body) = await Client.TestGet<DayOffRequestDetailsResponseDto>(
            user: theUser,
            route: Route.GetDayOff);

        // Assert
        body!.Requests.Select(x => x.Id).Should().BeEquivalentTo(new[] { 1.AsGuid(), 2.AsGuid(), 3.AsGuid(), 6.AsGuid() });
    }

}