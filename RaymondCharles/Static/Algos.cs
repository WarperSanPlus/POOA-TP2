namespace RaymondCharles.Static;

internal static class Algos
{
    internal static void PermuterObjets<T>(ref T obj1, ref T obj2) => (obj1, obj2) = (obj2, obj1);

    // Slightly faster than NumberUtils.MathFunctions.IsInBounds (0.0170 ns vs 1.0896 ns)
    // Extension pour faciliter l'utilisation
    internal static bool EstDansBornes(this int value, int min, int max) => value >= Math.Min(min, max) && value <= Math.Max(min, max);

    internal static bool AnyValid<T>(this IEnumerable<T> src, Predicate<T> condition)
    {
        foreach (var item in src)
            if (condition(item)) 
                return true;
        return false;
    }
}