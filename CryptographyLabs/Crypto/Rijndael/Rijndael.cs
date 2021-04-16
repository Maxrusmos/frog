using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.Crypto
{
    public static partial class Rijndael_
    {
        public static byte[] GenerateInvSBox()
        {
            byte[] sBox = GenerateSBox();
            return GenerateInvSBox(sBox);
        }

        public static byte[] GenerateSBox()
        {
            byte[] matrix = new byte[]
            {
                0b1111_0001,
                0b1110_0011,
                0b1100_0111,
                0b1000_1111,
                0b0001_1111,
                0b0011_1110,
                0b0111_1100,
                0b1111_1000
            };

            byte[] sBox = new byte[256];
            for (int i = 0; i < 256; ++i)
            {
                byte inv = GF.Inverse((byte)i);
                for (int j = 0; j < 8; j++)
                {
                    byte conj = (byte)(matrix[j] & inv);
                    byte xorSum = Bitops.XorBits(conj);
                    sBox[i] |= (byte)(xorSum << j);
                }
                sBox[i] ^= 0x63;
            }
            return sBox;
        }

        public static byte[] GenerateInvSBox(byte[] sBox)
        {
            byte[] invSBox = new byte[256];
            for (int i = 0; i < 256; ++i)
                invSBox[sBox[i]] = (byte)i;
            return invSBox;
        }

        public static byte[][] GenerateInvMixColumnsMtx()
        {
            return GFInverse(_mixColumnMatrix);
        }

        private static byte[][] GFInverse(byte[][] mtx)
        {
            byte det = GFDet(mtx);

            byte[][] result = new byte[mtx.Length][];
            for (int i = 0; i < mtx.Length; i++)
                result[i] = new byte[mtx.Length];

            for (int row = 0; row < mtx.Length; row++)
            {
                for (int col = 0; col < result[row].Length; col++)
                {
                    byte minor = GFMinor(mtx, row, col);
                    result[col][row] = GF.Divide(minor, det);
                }
            }

            return result;
        }

        private static byte GFMinor(byte[][] mtx, int row, int col)
        {
            if (mtx.Length == 2)
                return mtx[(row + 1) % 2][(col + 1) % 2];

            byte[][] subMatrix = SubMatrix(mtx, row, col);
            return GFDet(subMatrix);
        }

        private static byte GFDet(byte[][] mtx)
        {
            if (mtx.Length == 2)
            {
                byte tm = GF.Multiply(mtx[0][0], mtx[1][1]);
                tm ^= GF.Multiply(mtx[1][0], mtx[0][1]);
                return tm;
            }

            byte res = 0;
            for (int i = 0; i < mtx.Length; ++i)
            {
                byte detSub = GFDet(SubMatrix(mtx, i, 0));
                res ^= GF.Multiply(mtx[i][0], detSub);
            }
            return res;
        }

        private static byte[][] SubMatrix(byte[][] mtx, int row, int col)
        {
            byte[][] res = new byte[mtx.Length - 1][];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = new byte[mtx.Length - 1];
                int rowOffset = i < row ? 0 : 1;
                for (int j = 0; j < res[i].Length; j++)
                {
                    if (j < col)
                        res[i][j] = mtx[i + rowOffset][j];
                    else
                        res[i][j] = mtx[i + rowOffset][j + 1];
                }
            }
            return res;
        }


    }
}
