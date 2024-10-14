namespace Tf1DayOff.Infra.Helpers;

public static class HelperExtensions
{
    public static IDictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value,
        Func<TKey, TValue, TValue> update) where TKey : notnull
    {
        if (dict.ContainsKey(key))
        {
            dict[key] = update(key, dict[key]);
        }
        else
        {
            dict.Add(key, value);
        }

        return dict;
    }

    public static bool Matches<T>(this T x, T[]? filter)
    {
        return filter is null || !filter.Any() || filter.Contains(x);
    }
}