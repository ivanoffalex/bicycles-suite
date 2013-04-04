using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.UI.WebControls;

namespace BicyclesSuite.Shared
{
    /// <summary>
    /// Enum helper
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static class Enum<T> where T : struct, IConvertible
    {
        static Enum()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
        }

        /// <summary>
        /// Parse enum
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>enum</returns>
        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Get all enum values
        /// </summary>
        /// <returns>List of all enum value</returns>
        public static T[] ToArray()
        {
            List<T> list = new List<T>();
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                list.Add(item);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Convert flagged value to list
        /// </summary>
        /// <param name="value">Flagged value</param>
        /// <returns>List of flag value</returns>
        public static T[] ToArray(T value)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < Enum.GetNames(typeof(T)).Length; i++)
            {
                string item = Enum.GetNames(typeof(T))[i];
                T val = Parse(item);

                if (((Enum)(object)value).HasFlag((Enum)(object)val))
                {
                    list.Add(val);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Prepare ListItem list from specified enum
        /// </summary>
        /// <param name="useEnumValues"></param>
        /// <param name="replacementRules"></param>
        /// <returns></returns>
        public static List<ListItem> ToDropdown(bool useEnumValues = false, Tuple<string, string>[] replacementRules = null)
        {
            List<ListItem> result = new List<ListItem>();

            string text = null;
            string value = null;

            foreach (var enumValue in Enum.GetValues(typeof(T)))
            {
                value = useEnumValues ? ((int)enumValue).ToString() : enumValue.ToString();
                text = Enum.GetName(typeof(T), enumValue);

                if (replacementRules != null)
                {
                    foreach (Tuple<string, string> rule in replacementRules)
                    {
                        text = text.Replace(rule.Item1, rule.Item2);
                    }
                }

                ListItem lsItem = new ListItem
                {
                    Value = value,
                    Text = text
                };

                result.Add(lsItem);
            }

            return result;
        }

        
    }

}
