﻿using System.Collections.Generic;
using JetBrains.Annotations;

namespace RangedDPS.Utilities
{
    /// <summary>
    /// Utility methods for working with enumerables
    /// </summary>
    public static class EnumerableUtils
    {
        /// <summary>
        /// Yields the item as an enumerable, regardless of whether it's null or not
        /// </summary>
        /// <param name="item">Item.</param>
        [PublicAPI]
        public static IEnumerable<T?> Yield<T>(this T? item)
        {
            yield return item;
        }

        /// <summary>
        /// Yields the item as an enumerable.  If item is null, the enumerable will be empty
        /// </summary>
        /// <param name="item">Item.</param>
        [PublicAPI]
        public static IEnumerable<T> YieldIfNotNull<T>(this T? item)
        {
            if (item != null)
                yield return item;
        }
    }
}
