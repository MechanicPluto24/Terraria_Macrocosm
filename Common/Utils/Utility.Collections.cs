using System;
using System.Collections.Generic;
using System.Linq;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        /// <summary> Concatenates IEnumberable collections into a single collection</summary>
        public static IEnumerable<T> Concatenate<T>(params IEnumerable<T>[] collections)
            => collections.Aggregate((acc, collection) => acc.Concat(collection));

        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }
    }
}
