using System;
using System.Diagnostics;

namespace BicyclesSuite.Shared
{
    /// <summary>
    /// class SafeConvert contains methods for safe convert value
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static class SafeConvert
    {
        /// <summary>
        /// Default hex prefix
        /// </summary>
        private const string DEFAULT_HEX_PREFIX = "0x";

        #region Parse

        /// <summary>
        /// Parse value to Boolean with custom default value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns>Converted Boolean value</returns>
        public static bool ParseBoolean(string value, bool defaultValue = default(bool))
        {
            bool result;
            return Boolean.TryParse(value, out result) ? result : defaultValue;
        }

        /// <summary>
        /// Parse value to double with custom default value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns>Converted double value</returns>
        public static double ParseDouble(string value, double defaultValue = default(double))
        {
            double result;
            return Double.TryParse(value, out result) ? result : defaultValue;
        }

        /// <summary>
        /// Parse value to ulong with custom default value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns>Converted ulong value</returns>
        public static ulong ParseUInt64(string value, ulong defaultValue = default(ulong))
        {
            ulong result;
            return UInt64.TryParse(value, out result) ? result : defaultValue;
        }

        /// <summary>
        /// Parse value to int with custom default value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns>Converted int value</returns>
        public static int ParseInt32(string value, int defaultValue = default(int))
        {
            int result;
            return Int32.TryParse(value, out result) ? result : defaultValue;
        }

        /// <summary>
        /// Parse value to decimal with custom default value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns>Converted decimal value</returns>
        public static decimal ParseDecimal(string value, decimal defaultValue = default(decimal))
        {
            decimal result;
            return Decimal.TryParse(value, out result) ? result : defaultValue;
        }

        /// <summary>
        /// Parse value to DateTime with custom default value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns>Converted DateTime value</returns>
        public static DateTime ParseDateTime(string value, DateTime defaultValue = default(DateTime))
        {
            DateTime result;
            return DateTime.TryParse(value, out result) ? result : defaultValue;
        }

        #endregion

        #region Convert

        /// <summary>
        /// Convert value to Boolean with custom default value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Converted Boolean value</returns>
        public static bool ToBoolean(object value, bool defaultValue = default(bool))
        {
            if (!IsConvertibleObject(value))
            {
                return defaultValue;
            }
            try
            {
                return Convert.ToBoolean(value);
            }
            catch (InvalidCastException) { }
            catch (FormatException) { }
            catch (OverflowException) { }
            return defaultValue;
        }

        /// <summary>
        /// Convert value to int with custom default value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns>Converted int value</returns>
        public static int ToInt32(object value, int defaultValue = default(int))
        {
            if (!IsConvertibleObject(value))
            {
                return defaultValue;
            }
            try
            {
                return Convert.ToInt32(value);
            }
            catch (InvalidCastException) { }
            catch (FormatException) { }
            catch (OverflowException) { }
            return defaultValue;
        }

        /// <summary>
        /// Convert value to Decimal with custom default value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Converted decimal value</returns>
        public static decimal ToDecimal(object value, decimal defaultValue = default(decimal))
        {
            if (!IsConvertibleObject(value))
            {
                return defaultValue;
            }
            try
            {
                return Convert.ToDecimal(value);
            }
            catch (InvalidCastException) { }
            catch (FormatException) { }
            catch (OverflowException) { }
            return defaultValue;
        }

        /// <summary>
        /// Convert value to double with custom default value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Converted double value</returns>
        public static double ToDouble(object value, double defaultValue = default(double))
        {
            if (!IsConvertibleObject(value))
            {
                return defaultValue;
            }
            try
            {
                return Convert.ToDouble(value);
            }
            catch (InvalidCastException) { }
            catch (FormatException) { }
            catch (OverflowException) { }
            return defaultValue;
        }

        /// <summary>
        /// Convert value to ulong with custom default value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Converted ulong value</returns>
        public static ulong ToUInt64(object value, ulong defaultValue = default(ulong))
        {
            if (!IsConvertibleObject(value))
            {
                return defaultValue;
            }
            try
            {
                return Convert.ToUInt64(value);
            }
            catch (InvalidCastException) { }
            catch (FormatException) { }
            catch (OverflowException) { }
            return defaultValue;
        }

        /// <summary>
        /// Convert value to DateTime with custom default value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Converted DateTime value</returns>
        public static DateTime ToDateTime(object value, DateTime defaultValue = default(DateTime))
        {
            if (!IsConvertibleObject(value))
            {
                return defaultValue;
            }
            try
            {
                return Convert.ToDateTime(value);
            }
            catch (InvalidCastException) { }
            catch (FormatException) { }
            catch (OverflowException) { }
            return defaultValue;
        }

        #endregion

        #region Change Type methods

        /// <summary>
        /// Change object value to another type with FormatProvider
        /// </summary>
        /// <param name="value">Object value</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="conversionType">Conversion type</param>
        /// <param name="provider">Format provider</param>
        /// <returns>Converted object value</returns>
        public static object ChangeType(object value, object defaultValue, Type conversionType, IFormatProvider provider = null)
        {
            var result = defaultValue;
            try
            {
                result = Convert.ChangeType(value, conversionType, provider);
            }
            catch (InvalidCastException)
            {
            }
            return result;
        }

        /// <summary>
        /// Change object value to another type by TypeCode enum with FormatProvider
        /// </summary>
        /// <param name="value">Object value</param>
        /// <param name="defaultValue">default value</param>
        /// <param name="typeCode">TypeCode value</param>
        /// <param name="provider">FormatProvider</param>
        /// <returns>Converted object value</returns>
        public static object ChangeType(object value, object defaultValue, TypeCode typeCode, IFormatProvider provider = null)
        {
            var result = defaultValue;
            try
            {
                result = Convert.ChangeType(value, typeCode, provider);
            }
            catch (InvalidCastException)
            {
            }
            return result;
        }

        #endregion

        #region Special convertors

        /// <summary>
        /// Convert bytes array to hexadecimal string
        /// </summary>
        /// <param name="data">Bytes to convert</param>
        /// <param name="usePrefix">Use "0x" prefix to output string</param>
        /// <returns>Converted hexadecimal string</returns>
        public static string ToHex(byte[] data, bool usePrefix = false)
        {
            if (data == null || data.Length == 0) return string.Empty;
            var off = usePrefix ? 2 : 0;
            var c = new char[data.Length * 2 + off];
            if (usePrefix)
            {
                c[0] = DEFAULT_HEX_PREFIX[0];
                c[1] = DEFAULT_HEX_PREFIX[1];
            }
            var cnt = data.Length;
            for (var i = 0; i < cnt; i++)
            {
                var b = ((byte)(data[i] >> 4));
                c[i * 2 + off] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(data[i] & 0xF));
                c[i * 2 + 1 + off] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
            return new string(c);
        }

        /// <summary>
        /// Convert hexadecimal string to bytes array
        /// </summary>
        /// <param name="hex">Hexadecimal string</param>
        /// <returns>Bytes array</returns>
        public static byte[] FromHex(string hex)
        {
            if (string.IsNullOrEmpty(hex) || hex.Length % 2 == 1) return new byte[0];
            var first = new string(new[] { hex[0], hex[1] });
            var offset = string.Compare(first, DEFAULT_HEX_PREFIX, true) == 0 ? 2 : 0;
            var numberChars = hex.Length - offset;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i + offset, 2), 16);
            return bytes;
        }

        #endregion

        #region Private methods

        private static bool IsConvertibleObject(object obj)
        {
            if (obj == null || obj is DBNull || !(obj is IConvertible))
            {
                return false;
            }
            return !(obj is string) || ((string)obj) != string.Empty;
        }

        #endregion
    }

}
