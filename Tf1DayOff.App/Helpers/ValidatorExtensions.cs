namespace Tf1DayOff.App.Helpers;

public static class ValidatorExtensions
{
    public static string ShouldNotBeEmpty(this string prop)
    {
        return $"{prop} should not be empty";
    }
    public static string ShouldBeBefore(this string prop, string prop2)
    {
        return $"{prop} should not be before {prop2}";
    }
}