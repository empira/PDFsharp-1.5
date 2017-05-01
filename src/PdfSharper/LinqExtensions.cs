using System;
using System.Collections.Generic;

namespace PdfSharper
{
    public static class LinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> GroupWhile<T>(this IEnumerable<T> source, Func<T, T, bool> predicate)
        {
            using (var iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                    yield break;

                List<T> list = new List<T>() { iterator.Current };

                T previous = iterator.Current;

                while (iterator.MoveNext())
                {
                    if (predicate(previous, iterator.Current))
                    {
                        list.Add(iterator.Current);
                    }
                    else
                    {
                        yield return list;
                        list = new List<T>() { iterator.Current };
                    }

                    previous = iterator.Current;
                }
                yield return list;
            }
        }
    }
}
