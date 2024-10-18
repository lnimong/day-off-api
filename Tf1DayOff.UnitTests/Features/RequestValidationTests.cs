using FluentAssertions;
using System.Net;
using Tf1DayOff.Api.Controllers;
using Tf1DayOff.Api.Dtos;
using Tf1DayOff.Domain.Entities;
using Tf1DayOff.UnitTests.TestInitTools;

namespace Tf1DayOff.UnitTests.Features;

public class RequestValidationTests : BaseDayOFfTests
{

    [Test]
    public async Task The_Validation_Should_Update_The_Request_Status()
    {
        // Arrange
        var theUser = "u1";
        var theOtherUser = "u2";
        GivenTodayIs(Oct(10));
        GivenCurrentDayOffRequestAre(
            new DayOffRequest(1.AsGuid(), theUser, DayOffType.Vacation, Oct(2), Oct(6), DayOffRequestStatus.Pending),
            new DayOffRequest(2.AsGuid(), theUser, DayOffType.Vacation, Oct(10), Oct(16), DayOffRequestStatus.Pending),
            new DayOffRequest(3.AsGuid(), theUser, DayOffType.Vacation, Oct(20), Oct(26), DayOffRequestStatus.Pending)
        );

        // Act
        var response = await Client.TestPost(
            user: theOtherUser,
            route: Route.ValidateRequest(2.AsGuid()),
            body: new DayOffValidationRequestDto { Action = DayOffValidationAction.Validate });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Repository.Data[1.AsGuid()].Status.Should().Be(DayOffRequestStatus.Pending);
        Repository.Data[2.AsGuid()].Status.Should().Be(DayOffRequestStatus.Accepted);
        Repository.Data[3.AsGuid()].Status.Should().Be(DayOffRequestStatus.Pending);
        Repository.Data[2.AsGuid()].StatusUpdatedBy.Should().Be(theOtherUser);
    }

    [Test]
    public async Task The_Validation_Should_Fail_On_An_Already_Validated_Request()
    {
        // Arrange
        var theUser = "u1";
        var theOtherUser = "u2";
        GivenTodayIs(Oct(10));
        GivenCurrentDayOffRequestAre(
            new DayOffRequest(1.AsGuid(), theUser, DayOffType.Vacation, Oct(2), Oct(6), DayOffRequestStatus.Pending),
            new DayOffRequest(2.AsGuid(), theUser, DayOffType.Vacation, Oct(10), Oct(16), DayOffRequestStatus.Accepted),
            new DayOffRequest(3.AsGuid(), theUser, DayOffType.Vacation, Oct(20), Oct(26), DayOffRequestStatus.Pending)
        );

        // Act
        var response = await Client.TestPost(
            user: theOtherUser,
            route: Route.ValidateRequest(2.AsGuid()),
            body: new DayOffValidationRequestDto { Action = DayOffValidationAction.Validate });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
