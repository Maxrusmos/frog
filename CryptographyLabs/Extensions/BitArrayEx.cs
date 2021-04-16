using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CryptographyLabs.Extensions
{
    public static class BitArrayEx
    {
        public static string ToStringEx(this BitArray bitArray)
        {
            StringBuilder builder = new StringBuilder();
            foreach (bool b in bitArray)
            {
                builder.Append(b ? '1' : '0');
            }
            return builder.ToString();
        }

        public static int FirstIndexOf(this BitArray bitArray, bool value)
        {
            for (int i = 0; i < bitArray.Length; i++)
                if (bitArray[i] == value)
                    return i;
            return -1;
        }

        public static int FirstIndexOf(this BitArray bitArray, bool value, int startIndex)
        {
            for (int i = startIndex; i < bitArray.Length; i++)
                if (bitArray[i] == value)
                    return i;
            return -1;
        }
    }

}
