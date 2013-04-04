using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BicyclesSuite.Shared
{
    /// <summary>
    /// Extension methods for Collection classes
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static class CollectionExtension
    {
        /// <summary>
        /// Join each item in the collection together with the glue string, returning a single string
        /// Each object in the collection will be converted to string with ToString()
        /// </summary>
        /// <param name="list"></param>
        /// <param name="glue"></param>
        /// <returns></returns>
        public static string Join(this IList list, string glue)
        {
            int cnt = list != null ? list.Count : 0;
            StringBuilder sb = new StringBuilder();
            if (cnt > 0)
            {
                sb.Append(list[0]);
            }
            for (int i = 1; i < cnt; i++)
            {
                sb.Append(glue);
                sb.Append(list[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Join each item in the collection together with the glue string, returning a single string
        /// Each object in the collection will be converted to string with ToString()
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="glue"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable collection, string glue)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (object item in collection)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(glue);
                }
                sb.Append(item);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Split collection by chunks
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <param name="chunkSize"></param>
        public static void ActionByChunk<T>(this IEnumerable<T> source, Action<IEnumerable<T>> action, int chunkSize = 1000)
        {
            int count = source != null ? source.Count() : 0;
            int offset = 0;

            while (count > 0)
            {
                var items = source.Skip(offset).Take(chunkSize);
                action(items);
                offset += chunkSize;
                count -= chunkSize;
            }
        }

        /// <summary>
        /// Checks whether a collection is the same as another collection
        /// </summary>
        /// <param name="value">The current instance object</param>
        /// <param name="compareList">The collection to compare with</param>
        /// <param name="comparer">The comparer object to use to compare each item in the collection.  If null uses EqualityComparer(T).Default</param>
        /// <returns>True if the two collections contain all the same items in the same order</returns>
        public static bool IsEqualTo<TSource>(this IEnumerable<TSource> value, IEnumerable<TSource> compareList, IEqualityComparer<TSource> comparer)
        {
            if (value == compareList)
            {
                return true;
            }
            if (value == null || compareList == null)
            {
                return false;
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<TSource>.Default;
            }

            var enumerator1 = value.GetEnumerator();
            var enumerator2 = compareList.GetEnumerator();

            var enum1HasValue = enumerator1.MoveNext();
            var enum2HasValue = enumerator2.MoveNext();

            try
            {
                while (enum1HasValue && enum2HasValue)
                {
                    if (!comparer.Equals(enumerator1.Current, enumerator2.Current))
                    {
                        return false;
                    }

                    enum1HasValue = enumerator1.MoveNext();
                    enum2HasValue = enumerator2.MoveNext();
                }

                return !(enum1HasValue || enum2HasValue);
            }
            finally
            {
                enumerator1.Dispose();
                enumerator2.Dispose();
            }
        }

        /// <summary>
        /// Checks whether a collection is the same as another collection
        /// </summary>
        /// <param name="value">The current instance object</param>
        /// <param name="compareList">The collection to compare with</param>
        /// <returns>True if the two collections contain all the same items in the same order</returns>
        public static bool IsEqualTo<TSource>(this IEnumerable<TSource> value, IEnumerable<TSource> compareList)
        {
            return IsEqualTo(value, compareList, null);
        }

        /// <summary>
        /// Checks whether a collection is the same as another collection
        /// </summary>
        /// <param name="value">The current instance object</param>
        /// <param name="compareList">The collection to compare with</param>
        /// <returns>True if the two collections contain all the same items in the same order</returns>
        public static bool IsEqualTo(this IEnumerable value, IEnumerable compareList)
        {
            return IsEqualTo<object>((object[])value, (object[])compareList);
        }

        /// <summary>
        /// Compare arrays
        /// </summary>
        /// <typeparam name="TSource">Array type</typeparam>
        /// <param name="value">Array</param>
        /// <param name="compareArray">Compare array</param>
        /// <returns>Equality</returns>
        public static bool IsEqualArrayTo<TSource>(this TSource[] value, TSource[] compareArray)
        {
            var cnt = value != null ? value.Length : 0;
            var otherCnt = compareArray != null ? compareArray.Length : 0;
            if (cnt != otherCnt) return false;
            for (var i = 0; i < cnt; i++)
            {
                if (!value[i].Equals(compareArray[i])) return false;
            }
            return true;
        }


    }
}
