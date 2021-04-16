using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs
{
    public static class BinPoly
    {
        public static ushort Multiply(byte a, byte b)
        {
            ushort tm = b;
            ushort res = 0;
            do
            {
                if ((a & 1) == 1)
                    res ^= tm;
                a >>= 1;
                tm <<= 1;
            }
            while (a > 0);

            return res;
        }

        public static string ToAllStrRepr(ushort coefs)
        {
            return $"0b{Convert.ToString(coefs, 2)};      " +
                $"{coefs};      " +
                $"0x{Convert.ToString(coefs, 16)};      " +
                $"{StringEx.AsPolynom(coefs)};";
        }
    }
}
