using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitBucket.REST.API.Helpers
{
    public static class LinqExtensions
    {
        public static IEnumerable<TSource> Flatten<TSource>(
                 this IEnumerable<TSource> source,
                 Func<TSource, bool> selectorFunction,
                 Func<TSource, IEnumerable<TSource>> getChildrenFunction)
        {
            // Add what we have to the stack
            var flattenedList = source.Where(selectorFunction);

            // Go through the input enumerable looking for children,
            // and add those if we have them
            foreach (TSource element in source)
            {
                flattenedList = flattenedList.Concat(
                  getChildrenFunction(element).Flatten(selectorFunction,
                                                   getChildrenFunction)
                );
            }
            return flattenedList;
        }

        public static IEnumerable<int> FindAllIndexes<TSource>(this IEnumerable<TSource> source, Predicate<TSource> predicate)
        {
            int i = 0;
            foreach (var element in source)
            {
                if (predicate(element))
                    yield return i;

                i++;
            }
        }
    }
}
