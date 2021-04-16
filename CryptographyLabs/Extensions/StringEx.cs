using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace CryptographyLabs
{
    public static class StringEx
    {
        public static string AsPolynom(byte coefs)
        {
            return AsPolynom(coefs, 8);
        }

        public static string AsPolynom(ushort coefs)
        {
            return AsPolynom(coefs, 16);
        }

        private static string AsPolynom(ulong coefs, int bitsCount = sizeof(ulong))
        {
            StringBuilder builder = new StringBuilder();
            bool isFirst = true;
            for (int d = bitsCount - 1; d >= 0; d--)
            {
                if (((coefs >> d) & 1) == 0)
                    continue;

                if (isFirst)
                    isFirst = false;
                else
                    builder.Append(" + ");

                if (d == 0)
                    builder.Append("1");
                else if (d == 1)
                    builder.Append("x");
                else
                {
                    builder.Append("x^");
                    builder.Append(d);
                }
            }
            return builder.ToString();
        }

        public static bool TryParse(string strValue, out byte value)
        {
            strValue = strValue.Replace(" ", "").Replace("_", "");

            if (strValue.Length > 2 && strValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    value = Convert.ToByte(strValue.Substring(2), 16);
                    return true;
                }
                catch
                {
                    value = 0;
                    return false;
                }
            }
            else if (strValue.Length > 2 && strValue.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    value = Convert.ToByte(strValue.Substring(2), 2);
                    return true;
                }
                catch
                {
                    value = 0;
                    return false;
                }
            }
            else
            {
                return byte.TryParse(strValue, out value);
            }
        }

        public static bool TryParse(string strValue, out uint value)
        {
            strValue = strValue.Replace(" ", "").Replace("_", "");

            if (strValue.Length > 2 && strValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    value = Convert.ToUInt32(strValue.Substring(2), 16);
                    return true;
                }
                catch
                {
                    value = 0;
                    return false;
                }
            }
            else if (strValue.Length > 2 && strValue.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    value = Convert.ToUInt32(strValue.Substring(2), 2);
                    return true;
                }
                catch
                {
                    value = 0;
                    return false;
                }
            }
            else
            {
                return uint.TryParse(strValue, out value);
            }
        }

        public static bool TryParse(string strValue, out ulong value)
        {
            strValue = strValue.Replace(" ", "").Replace("_", "");

            if (strValue.Length > 2 && strValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    value = Convert.ToUInt64(strValue.Substring(2), 16);
                    return true;
                }
                catch
                {
                    value = 0;
                    return false;
                }
            }
            else if (strValue.Length > 2 && strValue.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    value = Convert.ToUInt64(strValue.Substring(2), 2);
                    return true;
                }
                catch
                {
                    value = 0;
                    return false;
                }
            }
            else
            {
                return ulong.TryParse(strValue, out value);
            }
        }

        public static bool TryParse(string strValue, out byte[] bytes)
        {
            if (TryParse(strValue, out BigInteger value))
            {
                bytes = value.ToByteArray();
                if (bytes.Length > 1 && bytes[bytes.Length - 1] == 0)
                    Array.Resize(ref bytes, bytes.Length - 1);
                return true;
            }
            else
            {
                bytes = null;
                return false;
            }
        }

        public static bool TryParse(string strValue, out BigInteger value)
        {
            strValue = strValue.Replace(" ", "").Replace("_", "");

            if (strValue.Length > 2 && strValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                strValue = strValue.Substring(2, strValue.Length - 2);
                return BigInteger.TryParse(strValue, NumberStyles.HexNumber, null, out value);
            }
            else if (strValue.Length > 2 && strValue.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
            {
                strValue = strValue.Substring(2, strValue.Length - 2);
                return TryParseBinary(strValue, out value);
            }
            else
                return BigInteger.TryParse(strValue, out value);
        }

        public static bool TryParseBinary(string strValue, out BigInteger value)
        {
            value = 0;
            foreach (char c in strValue)
            {
                value <<= 1;
                if (c == '1')
                    value |= 1;
                else if (c != '0')
                    return false;
            }
            return true;
        }

    }
}
