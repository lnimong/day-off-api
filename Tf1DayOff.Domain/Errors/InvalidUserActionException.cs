namespace Tf1DayOff.Domain.Errors;

public class InvalidUserActionException : Exception
{
    public InvalidUserActionException(string dayOffRequestOverlapsWithExistingDayOffTickets) : base(dayOffRequestOverlapsWithExistingDayOffTickets)
    { }
}