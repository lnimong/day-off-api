using System.Net;
using FluentAssertions;
using Tf1DayOff.Api.Dtos;
using Tf1DayOff.Domain.Entities;
using Tf1DayOff.UnitTests.TestInitTools;

namespace Tf1DayOff.UnitTests.Features;

public class RequestCreationTests : BaseDayOFfTests
{


    [TestCase(10, 14, 18, HttpStatusCode.OK)]
    [TestCase(19, 14, 18, HttpStatusCode.BadRequest)]
    [TestCase(15, 14, 18, HttpStatusCode.BadRequest)]
    [TestCase(14, 14, 18, HttpStatusCode.BadRequest)]
    public async Task The_TimeRange_Should_Be_After_The_Current_Date(int today, int start, int end, 
        HttpStatusCode expectedStatus)
    {
        // Arrange
        var theUser = "u1";
        GivenTodayIs(Oct(today));

        // Act
        var response = await Client.TestPost(
            user: theUser,
            route: Route.NewRequest,
            body: new DayOffRequestDto(DayOffType.Vacation, Oct(start), Oct(end)));

        // Assert
        response.StatusCode.Should().Be(expectedStatus);
    }

    [TestCase(14, 18, HttpStatusCode.OK)]
    [TestCase(18, 14, HttpStatusCode.BadRequest)]
    [TestCase(14, 14, HttpStatusCode.OK)]
    public async Task The_StartDate_Should_Be_Before_The_End_Date(int start, int end,
        HttpStatusCode expectedStatus)
    {
        // Arrange
        var theUser = "u1";
        GivenTodayIs(Oct(1));

        // Act
        var response = await Client.TestPost(
            user: theUser,
            route: Route.NewRequest,
            body: new DayOffRequestDto(DayOffType.Vacation, Oct(start), Oct(end)));

        // Assert
        response.StatusCode.Should().Be(expectedStatus);
    }

    [Test]
    public async Task The_TimeRange_Should_Not_Overlap_With_PreExisting_Requests_TimeRange()
    {
        // Arrange
        var theUser = "u1";
        GivenTodayIs(Oct(10));
        GivenCurrentDayOffRequestAre(
            new DayOffRequest(1.AsGuid(), theUser, DayOffType.Vacation, Oct(2), Oct(6), DayOffRequestStatus.Pending),
            new DayOffRequest(2.AsGuid(), theUser, DayOffType.Vacation, Oct(10), Oct(16), DayOffRequestStatus.Pending),
            new DayOffRequest(3.AsGuid(), theUser, DayOffType.Vacation, Oct(20), Oct(26), DayOffRequestStatus.Pending)
        );

        // Act
        var response = await Client.TestPost(
            user: theUser,
            route: Route.NewRequest,
            body: new DayOffRequestDto(DayOffType.Vacation, Oct(14), Oct(18)));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestCase( 2,  5, 10, 20, false)]
    [TestCase(21, 25, 10, 20, false)]
    [TestCase( 2, 15, 10, 20,  true)]
    [TestCase(11, 25, 10, 20,  true)]
    [TestCase( 2, 25, 10, 20,  true)]
    [TestCase(11, 15, 10, 20,  true)]
    public async Task The_TimeRange_Should_Not_Overlap_With_PreExisting_Requests_TimeRange(
        int newRequestStart, int newRequestEnd,
        int existingRequestStart, int existingRequestEnd,
        bool overlap)
    {
        // Arrange
        var theUser = "u1";
        GivenTodayIs(Oct(1));
        GivenCurrentDayOffRequestAre(
            new DayOffRequest(1.AsGuid(), theUser, DayOffType.Vacation, Oct(existingRequestStart), Oct(existingRequestEnd), DayOffRequestStatus.Pending)
        );

        // Act
        var response = await Client.TestPost(
            user: theUser,
            route: Route.NewRequest,
            body: new DayOffRequestDto(DayOffType.Vacation, Oct(newRequestStart), Oct(newRequestEnd)));

        // Assert
        response.StatusCode.Should().Be(overlap ? HttpStatusCode.BadRequest : HttpStatusCode.OK);
    }

}
