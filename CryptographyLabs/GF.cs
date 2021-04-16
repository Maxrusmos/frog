using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs
{
    public static class GF
    {
        private static readonly ushort _m = 0b1_0001_1011;
        private static byte[,] _divideMtx;
        private static byte[] _inverseVector;
        private static byte[,] _multiplyMtx;
        private const bool _defaultWithReplace = true;

        public static void InitReplaceMatrices()
        {
            CalcMultiplyMtx();
            CalcInverseVector();
            CalcDivideMatrix();
        }

        public static byte Divide(byte a, byte b, bool withReplaceMtx = _defaultWithReplace)
        {
            if (withReplaceMtx)
            {
                if (_divideMtx is null)
                    CalcDivideMatrix();
                return _divideMtx[a, b];
            }
            else
                return Divide_(a, b);
        }

        private static void CalcDivideMatrix()
        {
            if (_inverseVector is null)
                CalcInverseVector();

            _divideMtx = new byte[256, 256];
            for (int row = 0; row < 256; row++)
                for (int col = 0; col < 256; col++)
                    _divideMtx[row, col] = Multiply_((byte)row, _inverseVector[col]);
        }

        private static byte Divide_(byte a, byte b)
        {
            return Multiply_(a, Inverse_(b));
        }

        public static byte Inverse(byte a, bool withReplaceVector = _defaultWithReplace)
        {
            if (withReplaceVector)
            {
                if (_inverseVector is null)
                    CalcInverseVector();
                return _inverseVector[a];
            }
            else
                return Inverse_(a);
        }

        private static void CalcInverseVector()
        {
            _inverseVector = new byte[256];
            for (int i = 0; i < 256; i++)
                _inverseVector[i] = Inverse_((byte)i);
        }

        private static byte Inverse_(byte a)
        {
            if (a == 0)
                return 0;

            byte res = a;
            for (int i = 2; i <= 254; ++i)
                res = Multiply(res, a);
            return res;
        }

        public static byte Multiply(byte a, byte b, bool withReplaceMtx = _defaultWithReplace)
        {
            if (withReplaceMtx)
            {
                if (_multiplyMtx is null)
                    CalcMultiplyMtx();
                return _multiplyMtx[a, b];
            }
            else
                return Multiply_(a, b);
        }

        private static void CalcMultiplyMtx()
        {
            _multiplyMtx = new byte[256, 256];
            for (int row = 0; row < 256; row++)
                for (int col = 0; col < 256; col++)
                    _multiplyMtx[row, col] = Multiply_((byte)row, (byte)col);
        }

        private static byte Multiply_(byte a, byte b)
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
            return Mod(res);
        }

        public static byte Mod(ushort a)
        {
            int mDeg = 8;
            //int mDeg = DegreeOf(_m);
            ushort mod = a;

            while (true)
            {
                int degree = DegreeOf(mod);
                if (degree < mDeg)
                    break;
                int shift = degree - mDeg;
                mod ^= (ushort)(_m << shift);
            }
            return (byte)mod;
        }

        private static int DegreeOf(ushort gfValue)
        {
            int degree = 0;
            while (true)
            {
                gfValue >>= 1;
                if (gfValue == 0)
                    break;
                degree++;
            }
            return degree;
        }
    }
}
