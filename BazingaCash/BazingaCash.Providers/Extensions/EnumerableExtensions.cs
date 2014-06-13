using System;
using System.Collections.Generic;

namespace BazingaCash.Providers.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Process<T>(this IEnumerable<T> collection, Action<T> action)
        {
            var en = collection.GetEnumerator();
            while (en.MoveNext())
            {
                var current = en.Current;
                action(current);
                yield return current;
            }
        }
        public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            var en = collection.GetEnumerator();
            while (en.MoveNext())
            {
                var current = en.Current;
                action(current);
            }
        }
    }
}