using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.Crypto
{
    public static partial class Rijndael_
    {
        public enum Size
        {
            S128, S192, S256
        }

        public static int GetBytesCount(Size size)
        {
            switch(size)
            {
                default:
                case Size.S128:
                    return 16;
                case Size.S192:
                    return 24;
                case Size.S256:
                    return 32;
            }
        }

        private static int GetRoundsCount(Size stateSize, Size keySize)
        {
            if (stateSize == Size.S128 && keySize == Size.S128)
                return 10;
            else if (stateSize == Size.S256 || keySize == Size.S256)
                return 14;
            else
                return 12;
        }

        private static Size SizeByBytesCount(int bytesCount)
        {
            switch (bytesCount)
            {
                case 16:
                    return Size.S128;
                case 24:
                    return Size.S192;
                case 32:
                    return Size.S256;
                default:
                    throw new ArgumentException("Wrong bytes count.");
            }
        }

    }
}
