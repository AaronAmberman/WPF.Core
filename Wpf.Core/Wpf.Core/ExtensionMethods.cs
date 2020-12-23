using System.Collections;
using System.Collections.Generic;

namespace Wpf.Core
{
    public static class ExtensionMethods
    {
        #region ICollection<T>

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            foreach (T t in enumerable)
            {
                collection.Add(t);
            }
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            foreach (T t in enumerable)
            {
                collection.Remove(t);
            }
        }

        #endregion

        #region IList

        public static void AddRange<T>(this IList list, IEnumerable<T> enumerable)
        {
            foreach (T t in enumerable)
            {
                list.Add(t);
            }
        }

        public static void RemoveRange<T>(this IList list, IEnumerable<T> enumerable)
        {
            foreach (T t in enumerable)
            {
                list.Remove(t);
            }
        }

        #endregion
    }
}
