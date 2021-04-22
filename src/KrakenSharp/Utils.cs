using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KrakenSharp
{

    static class ConvertUtil
    {

        #region ToEnum

        public static T ToEnum<T>(string val, T defaultValue) where T : struct, System.IConvertible
        {
            if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

            try
            {
                T result = (T)System.Enum.Parse(typeof(T), val, true);
                return result;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T ToEnum<T>(int val, T defaultValue) where T : struct, System.IConvertible
        {
            if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

            try
            {
                return (T)System.Enum.ToObject(typeof(T), val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T ToEnum<T>(long val, T defaultValue) where T : struct, System.IConvertible
        {
            if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

            try
            {
                return (T)System.Enum.ToObject(typeof(T), val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T ToEnum<T>(object val, T defaultValue) where T : struct, System.IConvertible
        {
            return ToEnum<T>(System.Convert.ToString(val), defaultValue);
        }

        public static T ToEnum<T>(string val) where T : struct, System.IConvertible
        {
            return ToEnum<T>(val, default(T));
        }

        public static T ToEnum<T>(int val) where T : struct, System.IConvertible
        {
            return ToEnum<T>(val, default(T));
        }

        public static T ToEnum<T>(object val) where T : struct, System.IConvertible
        {
            return ToEnum<T>(System.Convert.ToString(val), default(T));
        }

        public static System.Enum ToEnumOfType(System.Type enumType, object value)
        {
            if (value == null)
                return System.Enum.ToObject(enumType, 0) as System.Enum;
            else if (IsNumeric(value))
                return System.Enum.ToObject(enumType, ToInt(value)) as System.Enum;
            else
                return System.Enum.Parse(enumType, System.Convert.ToString(value), true) as System.Enum;

        }

        public static bool TryToEnum<T>(object value, out T result) where T : struct, System.IConvertible
        {
            if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

            try
            {
                if (value == null)
                    result = (T)System.Enum.ToObject(typeof(T), 0);
                else if (IsNumeric(value))
                    result = (T)System.Enum.ToObject(typeof(T), ToInt(value));
                else
                    result = (T)System.Enum.Parse(typeof(T), System.Convert.ToString(value), true);
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }

        #endregion

        #region ConvertToUInt
        /// <summary>
        /// This will convert an integer to a uinteger. The negative integer value is treated as what the memory representation of that negative 
        /// value would be as a uinteger.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static uint ToUInt(sbyte value)
        {
            return System.Convert.ToUInt32(value);
        }

        public static uint ToUInt(byte value)
        {
            return System.Convert.ToUInt32(value);
        }

        public static uint ToUInt(short value)
        {
            return System.Convert.ToUInt32(value);
        }

        public static uint ToUInt(ushort value)
        {
            return System.Convert.ToUInt32(value);
        }

        public static uint ToUInt(int value)
        {
            return System.Convert.ToUInt32(value & 0xffffffffu);
        }

        public static uint ToUInt(uint value)
        {
            return value;
        }

        public static uint ToUInt(long value)
        {
            return System.Convert.ToUInt32(value & 0xffffffffu);
        }

        public static uint ToUInt(ulong value)
        {
            return System.Convert.ToUInt32(value & 0xffffffffu);
        }

        ////public static uint ToUInt(float value)
        ////{
        ////    return System.Convert.ToUInt32(value & 0xffffffffu);
        ////}

        ////public static uint ToUInt(double value)
        ////{
        ////    return System.Convert.ToUInt32(value & 0xffffffffu);
        ////}

        ////public static uint ToUInt(decimal value)
        ////{
        ////    return System.Convert.ToUInt32(value & 0xffffffffu);
        ////}

        public static uint ToUInt(bool value)
        {
            return (value) ? 1u : 0u;
        }

        public static uint ToUInt(char value)
        {
            return System.Convert.ToUInt32(value);
        }

        public static uint ToUInt(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToUInt32(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return ToUInt(value.ToString());
            }
        }

        public static uint ToUInt(string value, System.Globalization.NumberStyles style)
        {
            return ToUInt(ToDouble(value, style));
        }

        public static uint ToUInt(string value)
        {
            return ToUInt(ToDouble(value, System.Globalization.NumberStyles.Any));
        }
        #endregion

        #region ConvertToInt

        public static int ToInt(sbyte value)
        {
            return System.Convert.ToInt32(value);
        }

        public static int ToInt(byte value)
        {
            return System.Convert.ToInt32(value);
        }

        public static int ToInt(short value)
        {
            return System.Convert.ToInt32(value);
        }

        public static int ToInt(ushort value)
        {
            return System.Convert.ToInt32(value);
        }

        public static int ToInt(int value)
        {
            return value;
        }

        public static int ToInt(uint value)
        {
            if (value > int.MaxValue)
            {
                return int.MinValue + System.Convert.ToInt32(value & 0x7fffffff);
            }
            else
            {
                return System.Convert.ToInt32(value & 0xffffffff);
            }
        }

        public static int ToInt(long value)
        {
            if (value > int.MaxValue)
            {
                return int.MinValue + System.Convert.ToInt32(value & 0x7fffffff);
            }
            else
            {
                return System.Convert.ToInt32(value & 0xffffffff);
            }
        }

        public static int ToInt(ulong value)
        {
            if (value > int.MaxValue)
            {
                return int.MinValue + System.Convert.ToInt32(value & 0x7fffffff);
            }
            else
            {
                return System.Convert.ToInt32(value & 0xffffffff);
            }
        }

        public static int ToInt(float value)
        {
            return System.Convert.ToInt32(value);
            //if (value > int.MaxValue)
            //{
            //    return int.MinValue + System.Convert.ToInt32(value & 0x7fffffff);
            //}
            //else
            //{
            //    return System.Convert.ToInt32(value & 0xffffffff);
            //}
        }

        public static int ToInt(double value)
        {
            return System.Convert.ToInt32(value);
            //if (value > int.MaxValue)
            //{
            //    return int.MinValue + System.Convert.ToInt32(value & 0x7fffffff);
            //}
            //else
            //{
            //    return System.Convert.ToInt32(value & 0xffffffff);
            //}
        }

        public static int ToInt(decimal value)
        {
            return System.Convert.ToInt32(value);
            //if (value > int.MaxValue)
            //{
            //    return int.MinValue + System.Convert.ToInt32(value & 0x7fffffff);
            //}
            //else
            //{
            //    return System.Convert.ToInt32(value & 0xffffffff);
            //}
        }

        public static int ToInt(bool value)
        {
            return value ? 1 : 0;
        }

        public static int ToInt(char value)
        {
            return System.Convert.ToInt32(value);
        }

        public static int ToInt(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToInt32(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return ToInt(value.ToString());
            }
        }

        public static int ToInt(string value, System.Globalization.NumberStyles style)
        {
            return ToInt(ToDouble(value, style));
        }
        public static int ToInt(string value)
        {
            return ToInt(ToDouble(value, System.Globalization.NumberStyles.Any));
        }
        #endregion

        #region "ConvertToULong"
        /// <summary>
        /// This will System.Convert an integer to a uinteger. The negative integer value is treated as what the memory representation of that negative 
        /// value would be as a uinteger.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static ulong ToULong(sbyte value)
        {
            return System.Convert.ToUInt64(value);
        }

        public static ulong ToULong(byte value)
        {
            return System.Convert.ToUInt64(value);
        }

        public static ulong ToULong(short value)
        {
            return System.Convert.ToUInt64(value);
        }

        public static ulong ToULong(ushort value)
        {
            return System.Convert.ToUInt64(value);
        }

        public static ulong ToULong(int value)
        {
            return System.Convert.ToUInt64(value & long.MaxValue);
        }

        public static ulong ToULong(uint value)
        {
            return System.Convert.ToUInt64(value);
        }

        public static ulong ToULong(long value)
        {
            return System.Convert.ToUInt64(value & long.MaxValue);
        }

        public static ulong ToULong(ulong value)
        {
            return value;
        }

        ////public static ulong ToULong(float value)
        ////{
        ////    return System.Convert.ToUInt64(value & long.MaxValue);
        ////}

        ////public static ulong ToULong(double value)
        ////{
        ////    return System.Convert.ToUInt64(value & long.MaxValue);
        ////}

        ////public static ulong ToULong(decimal value)
        ////{
        ////    return System.Convert.ToUInt64(value & long.MaxValue);
        ////}

        public static ulong ToULong(bool value)
        {
            return (value) ? 1ul : 0ul;
        }

        public static ulong ToULong(char value)
        {
            return System.Convert.ToUInt64(value);
        }

        public static ulong ToULong(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToUInt64(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return ToULong(value.ToString());
            }
        }

        public static ulong ToULong(string value, System.Globalization.NumberStyles style)
        {
            return ToULong(ToDouble(value, style));
        }
        public static ulong ToULong(string value)
        {
            return ToULong(ToDouble(value, System.Globalization.NumberStyles.Any));
        }
        #endregion

        #region "ConvertToLong"
        public static long ToLong(sbyte value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(byte value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(short value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(ushort value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(int value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(uint value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(long value)
        {
            return value;
        }

        public static long ToLong(ulong value)
        {
            if (value > long.MaxValue)
            {
                return int.MinValue + System.Convert.ToInt32(value & long.MaxValue);
            }
            else
            {
                return System.Convert.ToInt64(value & long.MaxValue);
            }
        }

        ////public static long ToLong(float value)
        ////{
        ////    return System.Convert.ToInt64(value & long.MaxValue);
        ////}

        ////public static long ToLong(double value)
        ////{
        ////    return System.Convert.ToInt64(value & long.MaxValue);
        ////}

        ////public static long ToLong(decimal value)
        ////{
        ////    return System.Convert.ToInt64(value & long.MaxValue);
        ////}

        public static long ToLong(bool value)
        {
            return value ? 1 : 0;
        }

        public static long ToLong(char value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToInt64(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return ToLong(value.ToString());
            }
        }

        public static long ToLong(string value, System.Globalization.NumberStyles style)
        {
            return ToLong(ToDouble(value, style));
        }

        public static long ToLong(string value)
        {
            return ToLong(ToDouble(value, System.Globalization.NumberStyles.Any));
        }
        #endregion

        #region "ToSingle"
        public static float ToSingle(sbyte value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(byte value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(short value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(ushort value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(int value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(uint value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(long value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(ulong value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(float value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(double value)
        {
            return (float)value;
        }

        public static float ToSingle(decimal value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(bool value)
        {
            return value ? 1 : 0;
        }

        public static float ToSingle(char value)
        {
            return ToSingle(System.Convert.ToInt32(value));
        }

        public static float ToSingle(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToSingle(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return ToSingle(value.ToString());
            }
        }

        public static float ToSingle(string value, System.Globalization.NumberStyles style)
        {
            return System.Convert.ToSingle(ToDouble(value, style));
        }
        public static float ToSingle(string value)
        {
            return System.Convert.ToSingle(ToDouble(value, System.Globalization.NumberStyles.Any));
        }
        #endregion

        #region "ToDouble"
        public static double ToDouble(sbyte value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(byte value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(short value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(ushort value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(int value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(uint value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(long value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(ulong value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(float value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(double value)
        {
            return value;
        }

        public static double ToDouble(decimal value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(bool value)
        {
            return value ? 1 : 0;
        }

        public static double ToDouble(char value)
        {
            return ToDouble(System.Convert.ToInt32(value));
        }

        public static double ToDouble(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToDouble(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return ToDouble(value.ToString(), System.Globalization.NumberStyles.Any, null);
            }
        }

        /// <summary>
        /// System.Converts any string to a number with no errors.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="style"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        /// <remarks>
        /// TODO: I would also like to possibly include support for other number system bases. At least binary and octal.
        /// </remarks>
        public static double ToDouble(string value, System.Globalization.NumberStyles style, System.IFormatProvider provider)
        {
            if (string.IsNullOrEmpty(value)) return 0d;

            style = style & System.Globalization.NumberStyles.Any;
            double dbl = 0;
            if (double.TryParse(value, style, provider, out dbl))
            {
                return dbl;
            }
            else
            {
                //test hex
                int i;
                bool isNeg = false;
                for (i = 0; i < value.Length; i++)
                {
                    if (value[i] == ' ' || value[i] == '+') continue;
                    if (value[i] == '-')
                    {
                        isNeg = !isNeg;
                        continue;
                    }
                    break;
                }

                if (i < value.Length - 1 &&
                        (
                        (value[i] == '#') ||
                        (value[i] == '0' && (value[i + 1] == 'x' || value[i + 1] == 'X')) ||
                        (value[i] == '&' && (value[i + 1] == 'h' || value[i + 1] == 'H'))
                        ))
                {
                    //is hex
                    style = (style & System.Globalization.NumberStyles.HexNumber) | System.Globalization.NumberStyles.AllowHexSpecifier;

                    if (value[i] == '#') i++;
                    else i += 2;
                    int j = value.IndexOf('.', i);

                    if (j >= 0)
                    {
                        long lng = 0;
                        long.TryParse(value.Substring(i, j - i), style, provider, out lng);

                        if (isNeg)
                            lng = -lng;

                        long flng = 0;
                        string sfract = value.Substring(j + 1).Trim();
                        long.TryParse(sfract, style, provider, out flng);
                        return System.Convert.ToDouble(lng) + System.Convert.ToDouble(flng) / System.Math.Pow(16d, sfract.Length);
                    }
                    else
                    {
                        string num = value.Substring(i);
                        long l;
                        if (long.TryParse(num, style, provider, out l))
                            return System.Convert.ToDouble(l);
                        else
                            return 0d;
                    }
                }
                else
                {
                    return 0d;
                }
            }


            ////################
            ////OLD garbage heavy version

            //if (value == null) return 0d;
            //value = value.Trim();
            //if (string.IsNullOrEmpty(value)) return 0d;

            //#if UNITY_WEBPLAYER
            //			Match m = Regex.Match(value, RX_ISHEX, RegexOptions.IgnoreCase);
            //#else
            //            Match m = Regex.Match(value, RX_ISHEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //#endif

            //if (m.Success)
            //{
            //    long lng = 0;
            //    style = (style & System.Globalization.NumberStyles.HexNumber) | System.Globalization.NumberStyles.AllowHexSpecifier;
            //    long.TryParse(m.Groups["num"].Value, style, provider, out lng);

            //    if (m.Groups["sign"].Value == "-")
            //        lng = -lng;

            //    if (m.Groups["fractional"].Success)
            //    {
            //        long flng = 0;
            //        string sfract = m.Groups["fractional"].Value.Substring(1);
            //        long.TryParse(sfract, style, provider, out flng);
            //        return System.Convert.ToDouble(lng) + System.Convert.ToDouble(flng) / System.Math.Pow(16d, sfract.Length);
            //    }
            //    else
            //    {
            //        return System.Convert.ToDouble(lng);
            //    }

            //}
            //else
            //{
            //    style = style & System.Globalization.NumberStyles.Any;
            //    double dbl = 0;
            //    double.TryParse(value, style, provider, out dbl);
            //    return dbl;

            //}
        }

        public static double ToDouble(string value, System.Globalization.NumberStyles style)
        {
            return ToDouble(value, style, null);
        }

        public static double ToDouble(string value)
        {
            return ToDouble(value, System.Globalization.NumberStyles.Any, null);
        }
        #endregion

        #region "ToDecimal"
        public static decimal ToDecimal(sbyte value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(byte value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(short value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(ushort value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(int value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(uint value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(long value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(ulong value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(float value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(double value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(decimal value)
        {
            return value;
        }

        public static decimal ToDecimal(bool value)
        {
            return value ? 1 : 0;
        }

        public static decimal ToDecimal(char value)
        {
            return ToDecimal(System.Convert.ToInt32(value));
        }

        public static decimal ToDecimal(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToDecimal(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return ToDecimal(value.ToString());
            }
        }

        public static decimal ToDecimal(string value, System.Globalization.NumberStyles style)
        {
            return System.Convert.ToDecimal(ToDouble(value, style));
        }
        public static decimal ToDecimal(string value)
        {
            return System.Convert.ToDecimal(ToDouble(value, System.Globalization.NumberStyles.Any));
        }
        #endregion

        #region "ToBool"
        public static bool ToBool(sbyte value)
        {
            return value != 0;
        }

        public static bool ToBool(byte value)
        {
            return value != 0;
        }

        public static bool ToBool(short value)
        {
            return value != 0;
        }

        public static bool ToBool(ushort value)
        {
            return value != 0;
        }

        public static bool ToBool(int value)
        {
            return value != 0;
        }

        public static bool ToBool(uint value)
        {
            return value != 0;
        }

        public static bool ToBool(long value)
        {
            return value != 0;
        }

        public static bool ToBool(ulong value)
        {
            return value != 0;
        }

        public static bool ToBool(float value)
        {
            return value != 0;
        }

        public static bool ToBool(double value)
        {
            return value != 0;
        }

        public static bool ToBool(decimal value)
        {
            return value != 0;
        }

        public static bool ToBool(bool value)
        {
            return value;
        }

        public static bool ToBool(char value)
        {
            return System.Convert.ToInt32(value) != 0;
        }

        public static bool ToBool(object value)
        {
            if (value == null)
            {
                return false;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToBoolean(value);
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return ToBool(value.ToString());
            }
        }

        /// <summary>
        /// Converts a string to boolean. Is FALSE greedy.
        /// A string is considered TRUE if it DOES meet one of the following criteria:
        /// 
        /// doesn't read blank: ""
        /// doesn't read false (not case-sensitive)
        /// doesn't read 0
        /// doesn't read off (not case-sensitive)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool ToBool(string str)
        {
            //str = (str + "").Trim().ToLower();
            //return !System.Convert.ToBoolean(string.IsNullOrEmpty(str) || str == "false" || str == "0" || str == "off");

            return !(string.IsNullOrEmpty(str) || str.Equals("false", System.StringComparison.OrdinalIgnoreCase) || str.Equals("0", System.StringComparison.OrdinalIgnoreCase) || str.Equals("off", System.StringComparison.OrdinalIgnoreCase));
        }


        public static bool ToBoolInverse(sbyte value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(byte value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(short value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(ushort value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(int value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(uint value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(long value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(ulong value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(float value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(double value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(decimal value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(bool value)
        {
            return value;
        }

        public static bool ToBoolInverse(char value)
        {
            return System.Convert.ToInt32(value) != 0;
        }

        public static bool ToBoolInverse(object value)
        {
            if (value == null)
            {
                return false;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToBoolean(value);
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return ToBoolInverse(value.ToString());
            }
        }

        /// <summary>
        /// Converts a string to boolean. Is TRUE greedy (inverse of ToBool)
        /// A string is considered TRUE if it DOESN'T meet any of the following criteria:
        /// 
        /// reads blank: ""
        /// reads false (not case-sensitive)
        /// reads 0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool ToBoolInverse(string str)
        {
            //str = (str + "").Trim().ToLower();
            //return (!string.IsNullOrEmpty(str) && str != "false" && str != "0");

            return !string.IsNullOrEmpty(str) &&
                   !str.Equals("false", System.StringComparison.OrdinalIgnoreCase) &&
                   !str.Equals("0", System.StringComparison.OrdinalIgnoreCase) &&
                   !str.Equals("off", System.StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region "Time/Date"

        /// <summary>
        /// Converts an object value to a date value first by straight conversion then by changing it to string if the 
        /// straight conversion doesn't work
        /// </summary>
        /// <param name="value">Object vale</param>
        /// <returns>Date</returns>
        /// <remarks></remarks>
        public static System.DateTime ToDate(object value)
        {
            try
            {
                //'try straight convert
                return System.Convert.ToDateTime(value);

            }
            catch
            {
            }

            try
            {
                //'if straight convert failed, try by string
                return System.Convert.ToDateTime(System.Convert.ToString(value));

            }
            catch
            {
            }

            //'if all fail, return Date(0)
            return new System.DateTime(0);
        }

        public static System.DateTime ToDate(object day, object secondsIntoDay)
        {
            return ConvertUtil.ToDate(day).Date + ConvertUtil.ToTime(secondsIntoDay);
        }

        /// <summary>
        /// Converts the input to the time of day. If value is numeric it is considered as seconds into the day.
        /// </summary>
        /// <param name="value">Object Value treated as number of seconds in the day</param>
        /// <returns>TimeSpan Value</returns>
        /// <remarks>
        /// If input is a TimeSpan, that TimeSpan is just returned.
        /// 
        /// If input is numeric, it is considered as seconds into the day.
        /// 
        /// If value is formatted as LoD supported time format (i.e. '5 seconds', '3.2 days', '100 ticks', etc), it is converted accordingly
        /// 
        /// Lastly it'll attempt to parse to a TimeSpan using normal .Net TimeSpan formatting with system region.
        /// 
        /// If all else, TimeSpan.Zero is returned.
        /// 
        /// 
        /// Supported LoD time formats:
        /// Ticks (any decimal will be rounded off):
        /// #.# t
        /// #.# tick
        /// #.# ticks
        /// 
        /// Milliseconds:
        /// #.# ms
        /// #.# millisecond
        /// #.# milliseconds
        /// 
        /// Seconds:
        /// #.# s
        /// #.# sec
        /// #.# secs
        /// #.# seconds
        /// #.# seconds
        /// 
        /// Minutes:
        /// #.# m
        /// #.# min
        /// #.# mins
        /// #.# minute
        /// #.# minutes
        /// 
        /// Hours:
        /// #.# h
        /// #.# hour
        /// #.# hours
        /// 
        /// Days:
        /// #.# d
        /// #.# day
        /// #.# days
        /// </remarks>
        public static System.TimeSpan ToTime(object value)
        {
            const string RX_TIME = "^\\d+(\\.\\d+)?\\s+((t)|(tick)|(ticks)|(ms)|(millisecond)|(milliseconds)|(s)|(sec)|(secs)|(second)|(seconds)|(m)|(min)|(mins)|(minute)|(minutes)|(h)|(hour)|(hours)|(d)|(day)|(days))$";

            if (value is System.TimeSpan)
            {
                return (System.TimeSpan)value;
            }
            else
            {
                if (IsNumeric(value))
                {
                    return System.TimeSpan.FromSeconds(ToDouble(value));
                }
                else
                {
                    var sval = System.Convert.ToString(value);
#if UNITY_WEBPLAYER
					if (sval != null && Regex.IsMatch(sval.Trim(), RX_TIME, RegexOptions.IgnoreCase))
#else
                    if (sval != null && Regex.IsMatch(sval.Trim(), RX_TIME, RegexOptions.IgnoreCase | RegexOptions.Compiled))
#endif
                    {
                        sval = Regex.Replace(sval.Trim(), "\\s+", " ");
                        var arr = sval.Split(' ');
                        switch (arr[1].ToLower())
                        {
                            case "t":
                            case "tick":
                            case "ticks":
                                return System.TimeSpan.FromTicks(ConvertUtil.ToLong(arr[0]));
                            case "ms":
                            case "millisecond":
                            case "milliseconds":
                                return System.TimeSpan.FromMilliseconds(ConvertUtil.ToDouble(arr[0]));
                            case "s":
                            case "sec":
                            case "secs":
                            case "second":
                            case "seconds":
                                return System.TimeSpan.FromSeconds(ConvertUtil.ToDouble(arr[0]));
                            case "m":
                            case "min":
                            case "mins":
                            case "minute":
                            case "minutes":
                                return System.TimeSpan.FromMinutes(ConvertUtil.ToDouble(arr[0]));
                            case "h":
                            case "hour":
                            case "hours":
                                return System.TimeSpan.FromHours(ConvertUtil.ToDouble(arr[0]));
                            case "d":
                            case "day":
                            case "days":
                                return System.TimeSpan.FromDays(ConvertUtil.ToDouble(arr[0]));
                            default:
                                return System.TimeSpan.Zero;
                        }
                    }
                    else
                    {
                        System.TimeSpan result = default(System.TimeSpan);
                        if (System.TimeSpan.TryParse(sval, out result))
                        {
                            return result;
                        }
                        else
                        {
                            return System.TimeSpan.Zero;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Returns number of seconds into the day a timeofday is. Acts similar to 'TimeToJulian' from old Dockmaster.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int TimeOfDayToSeconds(object value)
        {
            if (value is System.TimeSpan)
            {
                return (int)((System.TimeSpan)value).TotalSeconds;
            }
            else if (value is System.DateTime)
            {
                return (int)((System.DateTime)value).TimeOfDay.TotalSeconds;

            }
            else
            {
                try
                {
                    return (int)System.DateTime.Parse(ConvertUtil.ToString(value)).TimeOfDay.TotalSeconds;
                }
                catch
                {
                    return 0;
                }

            }

        }

        public static double TimeOfDayToMinutes(object value)
        {
            if (value is System.TimeSpan)
            {
                return ((System.TimeSpan)value).TotalMinutes;
            }
            else if (value is System.DateTime)
            {
                return ((System.DateTime)value).TimeOfDay.TotalMinutes;

            }
            else
            {
                try
                {
                    return System.DateTime.Parse(ConvertUtil.ToString(value)).TimeOfDay.TotalMinutes;
                }
                catch
                {
                    return 0;
                }

            }

        }

        public static double TimeOfDayToHours(object value)
        {
            if (value is System.TimeSpan)
            {
                return ((System.TimeSpan)value).TotalHours;
            }
            else if (value is System.DateTime)
            {
                return ((System.DateTime)value).TimeOfDay.TotalHours;

            }
            else
            {
                try
                {
                    return System.DateTime.Parse(ConvertUtil.ToString(value)).TimeOfDay.TotalHours;
                }
                catch
                {
                    return 0;
                }

            }

        }

        #endregion

        #region "Object Only odd prims, TODO"

        public static sbyte ToSByte(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return System.Convert.ToSByte(ToInt(value.ToString()) & 0x7f);
            }
        }

        public static byte ToByte(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return System.Convert.ToByte(ToInt(value.ToString()) & 0xff);
            }
        }

        public static short ToShort(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return System.Convert.ToInt16(ToInt(value.ToString()) & 0x7fff);
            }
        }

        public static System.UInt16 ToUShort(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return System.Convert.ToUInt16(ToInt(value.ToString()) & 0xffff);
            }
        }

        public static char ToChar(object value)
        {
            try
            {
                return System.Convert.ToChar(value);

            }
            catch (System.Exception)
            {
            }

            return System.Char.Parse("");
        }

        #endregion

        #region "ToString"

        public static string ToString(sbyte value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(byte value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(short value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(ushort value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(int value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(uint value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(long value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(ulong value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(float value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(double value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(decimal value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(bool value, string sFormat)
        {
            switch (sFormat)
            {
                case "num":
                    return (value) ? "1" : "0";
                case "normal":
                case "":
                case null:
                    return System.Convert.ToString(value);
                default:
                    return System.Convert.ToString(value);
            }
        }

        public static string ToString(bool value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(char value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(object value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(string str)
        {
            return str;
        }

        #endregion

        #region IsSupported

        /// <summary>
        /// Returns true if the object could be converted to a number by ConvertUtil
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsNumeric(object value, System.Globalization.NumberStyles style = System.Globalization.NumberStyles.Any, System.IFormatProvider provider = null, bool bBlankIsZero = false)
        {
            if (value == null) return bBlankIsZero;
            if (ValueIsNumericType(value)) return true;

            string sval = System.Convert.ToString(value);
            if (string.IsNullOrEmpty(sval))
                return bBlankIsZero;

            sval = sval.Trim();

            if (IsHex(sval))
            {
                return true;
            }
            else
            {
                style = style & System.Globalization.NumberStyles.Any;
                double dbl = 0;
                return double.TryParse(sval, style, provider, out dbl);
            }
        }

        public static bool IsHex(string value)
        {
            int i;
            for (i = 0; i < value.Length; i++)
            {
                if (value[i] == ' ' || value[i] == '+' || value[i] == '-') continue;

                break;
            }

            return (i < value.Length - 1 &&
                    (
                    (value[i] == '#') ||
                    (value[i] == '0' && (value[i + 1] == 'x' || value[i + 1] == 'X')) ||
                    (value[i] == '&' && (value[i + 1] == 'h' || value[i + 1] == 'H'))
                    ));
        }

        /// <summary>
        /// Tests if the typeof the value passed in is a numeric type. Handles IWrapper types as well.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool ValueIsNumericType(object obj)
        {
            if (obj == null) return false;

            var tp = obj.GetType();
            return tp.IsEnum || IsNumericType(System.Type.GetTypeCode(tp));
        }

        /// <summary>
        /// Tests if the type is a numeric type.
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsNumericType(System.Type tp)
        {
            if (tp == null) return false;
            return tp.IsEnum || IsNumericType(System.Type.GetTypeCode(tp));
        }

        public static bool IsNumericType(System.TypeCode code)
        {
            switch (code)
            {
                case System.TypeCode.SByte:
                    //5
                    return true;
                case System.TypeCode.Byte:
                    //6
                    return true;
                case System.TypeCode.Int16:
                    //7
                    return true;
                case System.TypeCode.UInt16:
                    //8
                    return true;
                case System.TypeCode.Int32:
                    //9
                    return true;
                case System.TypeCode.UInt32:
                    //10
                    return true;
                case System.TypeCode.Int64:
                    //11
                    return true;
                case System.TypeCode.UInt64:
                    //12
                    return true;
                case System.TypeCode.Single:
                    //13
                    return true;
                case System.TypeCode.Double:
                    //14
                    return true;
                case System.TypeCode.Decimal:
                    //15

                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// If type passed is a support TypeCode
        /// </summary>
        /// <param name="tp">Type</param>
        /// <returns>Returns true is type given is a supported type</returns>
        /// <remarks></remarks>
        public static bool IsSupportedType(System.Type tp)
        {
            System.TypeCode code = System.Type.GetTypeCode(tp);
            return IsSupportedType(tp, code);
        }

        private static bool IsSupportedType(System.Type tp, System.TypeCode code)
        {
            if (tp != null && tp.IsEnum) return true;

            if (code == System.TypeCode.Object)
            {
                if (tp == typeof(object)) return true;
                else if (tp == typeof(System.TimeSpan)) return true;
                else return false;
            }
            else
            {
                return !(code < 0 || (int)code > 18 || (int)code == 17);
            }
        }

        #endregion

        #region ToPrim

        /// <summary>
        /// Converts value to a Prim type of "T"
        /// </summary>
        /// <typeparam name="T">Prim type to be converted to</typeparam>
        /// <param name="value">Object value to be converted</param>
        /// <returns>Value as new converted type</returns>
        /// <remarks></remarks>
        public static T ToPrim<T>(object value)
        {
            if (value is T) return (T)value;

            System.Type tp = typeof(T);
            System.TypeCode code = System.Type.GetTypeCode(tp);
            if (!ConvertUtil.IsSupportedType(tp, code))
            {
                throw new System.Exception(typeof(T).Name + " is not accepted as a generic type for ConvertUtil.ToPrim.");
            }

            return (T)ConvertUtil.ToPrim(value, tp, code);
        }

        public static object ToPrim(object value, System.Type tp)
        {
            if (tp == null) throw new System.ArgumentException("Type must be non-null", "tp");
            if (value != null && tp.IsInstanceOfType(value)) return value;

            System.TypeCode code = System.Type.GetTypeCode(tp);
            if (!ConvertUtil.IsSupportedType(tp, code))
            {
                throw new System.Exception(tp.Name + " is not accepted as a type for ConvertUtil.ToPrim.");
            }

            return ConvertUtil.ToPrim(value, tp, code);
        }

        public static bool TryToPrim<T>(object value, out T output)
        {
            try
            {
                output = ToPrim<T>(value);
                return true;

            }
            catch
            {
                output = default(T);
            }

            return false;
        }

        public static bool TryToPrim(object value, System.Type tp, out object output)
        {
            try
            {
                output = ToPrim(value, tp);
                return true;
            }
            catch
            {
                output = null;
            }

            return false;
        }

        public static object ToPrim(object value, System.TypeCode code)
        {
            if (System.Convert.GetTypeCode(value) == code) return value;
            return ToPrim(value, null, code);
        }

        private static object ToPrim(object value, System.Type tp, System.TypeCode code)
        {
            //first make sure it's not an enum
            if (tp != null && tp.IsEnum)
            {
                if (value is string)
                    return System.Enum.Parse(tp, value as string, true);
                else if (value == null)
                    value = 0;

                switch (System.Type.GetTypeCode(value.GetType()))
                {
                    case System.TypeCode.SByte:
                    case System.TypeCode.Byte:
                    case System.TypeCode.Int16:
                    case System.TypeCode.UInt16:
                    case System.TypeCode.Int32:
                    case System.TypeCode.UInt32:
                    case System.TypeCode.Int64:
                    case System.TypeCode.UInt64:
                        return System.Enum.ToObject(tp, value);
                    default:
                        return System.Enum.Parse(tp, System.Convert.ToString(value), true);
                }
            }

            //now base off of the TypeCode
            switch (code)
            {
                case System.TypeCode.Empty:
                    //0
                    return null;
                case System.TypeCode.Object:
                    //1
                    if (tp == null || object.ReferenceEquals(tp, typeof(object))) return value;
                    else if (tp == typeof(System.TimeSpan)) return ConvertUtil.ToTime(value);
                    else return null;

                case System.TypeCode.DBNull:
                    //2
                    return System.DBNull.Value;
                case System.TypeCode.Boolean:
                    //3
                    return ConvertUtil.ToBool(value);
                case System.TypeCode.Char:
                    //4
                    return ConvertUtil.ToChar(value);
                case System.TypeCode.SByte:
                    //5
                    return ConvertUtil.ToSByte(value);
                case System.TypeCode.Byte:
                    //6
                    return ConvertUtil.ToByte(value);
                case System.TypeCode.Int16:
                    //7
                    return ConvertUtil.ToShort(value);
                case System.TypeCode.UInt16:
                    //8
                    return ConvertUtil.ToUShort(value);
                case System.TypeCode.Int32:
                    //9
                    return ConvertUtil.ToInt(value);
                case System.TypeCode.UInt32:
                    //10
                    return ConvertUtil.ToUInt(value);
                case System.TypeCode.Int64:
                    //11
                    return ConvertUtil.ToLong(value);
                case System.TypeCode.UInt64:
                    //12
                    return ConvertUtil.ToULong(value);
                case System.TypeCode.Single:
                    //13
                    return ConvertUtil.ToSingle(value);
                case System.TypeCode.Double:
                    //14
                    return ConvertUtil.ToDouble(value);
                case System.TypeCode.Decimal:
                    //15
                    return ConvertUtil.ToDecimal(value);
                case System.TypeCode.DateTime:
                    //16
                    return ConvertUtil.ToDate(value);
                case System.TypeCode.String:
                    //18
                    return System.Convert.ToString(value);
                default:
                    return null;
            }
        }

        #endregion

    }

    public static class SecureMessaging
    {

        /// <summary>
        /// Converts a <see cref="SecureString"/> instance to plain <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">value</exception>
        public static string ToPlainString(this SecureString value)
        {
#pragma warning disable S1854 // Unused assignments should be removed
            IntPtr unmanagedString = IntPtr.Zero;
#pragma warning restore S1854 // Unused assignments should be removed
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(value ?? throw new ArgumentNullException(nameof(value)));
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Converts to a <see cref="SecureString"/> instance.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">value</exception>
        public static SecureString ToSecureString(this string value)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            var secureString = new SecureString();
            secureString.Clear();

            foreach (var character in value)
            {
                secureString.AppendChar(character);
            }
            secureString.MakeReadOnly();

            return secureString;
        }

        public static byte[] CalculateSignature(byte[] props, long nonce, string path, SecureString apiSecret)
        {
            var decodedSecret = Convert.FromBase64String(apiSecret.ToPlainString());

            var np = Encoding.UTF8.GetBytes((nonce + Convert.ToChar(0)).ToString()).Concat(props).ToArray();
            var hash256Bytes = SHA256Hash(np);

            var pathBytes = Encoding.UTF8.GetBytes(path);

            var z = pathBytes.Concat(hash256Bytes).ToArray();

            var signature = getHash(decodedSecret, z);

            return signature;
        }

        private static byte[] SHA256Hash(byte[] value)
        {
            using (var hash = SHA256.Create())
            {
                return hash.ComputeHash(value);
            }
        }

        private static byte[] getHash(byte[] keyByte, byte[] messageBytes)
        {
            using (var hmacsha512 = new HMACSHA512(keyByte))
            {
                var result = hmacsha512.ComputeHash(messageBytes);
                return result;
            }
        }

    }


    /*
     * Significant portions of this code was based on code authored by Maik (github user m4cx)
     * in the repository found at:
     * https://github.com/m4cx/kraken-wsapi-dotnet
     * 
     * This code was modifed/refactored based under the permissiveness of the MIT License:
     * 
     * Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
     * and associated documentation files (the "Software"), to deal in the Software without restriction, 
     * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
     * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
     * subject to the following conditions:
     * The above copyright notice and this permission notice shall be included in all copies or substantial 
     * portions of the Software.
     * 
     * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
     * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
     * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
     * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
     * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
     */
    internal static class KrakenDataMessageHelper
    {
        /// <summary>
        /// Ensures the raw message.
        /// </summary>
        /// <param name="rawMessage">The raw message.</param>
        /// <returns>The raw message as a <see cref="JArray"/> instance</returns>
        /// <exception cref="ArgumentNullException">rawMessage</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static JArray EnsureRawMessageIsJArray(string rawMessage)
        {
            if (rawMessage == null)
            {
                throw new ArgumentNullException(nameof(rawMessage));
            }

            if (string.IsNullOrEmpty(rawMessage))
            {
                throw new ArgumentOutOfRangeException(nameof(rawMessage));
            }

            try
            {
                var token = JToken.Parse(rawMessage);
                if (!(token is JArray))
                {
                    throw new ArgumentOutOfRangeException(nameof(rawMessage));
                }

                return token as JArray;
            }
            catch (JsonReaderException ex)
            {
                throw new ArgumentOutOfRangeException(nameof(rawMessage), ex.Message);
            }
        }
    }

}
