using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BicyclesSuite.Shared
{
    /// <summary>
    /// Extension methods for string working
    /// </summary>
    public static class StringExtension
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
        /// Split strings into list of simple objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="separators"></param>
        /// <returns></returns>
        public static IEnumerable<T> Split<T>(this string values, char[] separators) where T : IConvertible
        {
            string[] source = values.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            T[] destination = new T[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                destination[i] = (T)Convert.ChangeType(source[i], typeof(T));
            }
            return destination;
        }
    }
}
