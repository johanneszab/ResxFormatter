using System;
using System.Collections.Generic;
using System.Reflection;

namespace ResxFormatter
{
    public class DelegateComparer<T> : IComparer<T>
    {
        private readonly Func<T?, T?, int> comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateComparer{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public DelegateComparer(Func<T?, T?, int> comparer)
        {
            this.comparer = comparer;
        }

        /// <inheritdoc />
        public int Compare(T? x, T? y)
        {
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
                if (x is null)
                    return y is null ? 0 : -1;

                if (y is null)
                    return 1;
            }

            return comparer(x, y);
        }
    }
}