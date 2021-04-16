using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs
{
    public static class Bitops
    {
        private static byte[] _deltas64 = new byte[] { 1, 2, 4, 8, 16, 32, 16, 8, 4, 2, 1 };

        public static ulong SwapBitsMasks64(ulong value, ulong[] masks)
        {
            if (_deltas64.Length != masks.Length)
                throw new ArgumentException($"Count of masks must be {_deltas64.Length}");

            for (int i = 0; i < _deltas64.Length; ++i)
            {
                ulong mask = masks[i];
                if (mask != 0)
                    value = SwapBitsMask(value, _deltas64[i], mask);
            }
            return value;
        }

        public static ulong SwapBitsMask(ulong x, int delta, ulong mask)
        {
            // page 177 (69)
            ulong y = (x ^ (x >> delta)) & mask;
            return x ^ y ^ (y << delta);
        }

        public static uint SwapBitsMask(uint x, int delta, uint mask)
        {
            uint y = (x ^ (x >> delta)) & mask;
            return x ^ y ^ (y << delta);
        }

        // simple tasks
        // Task11
        public static byte GetKthBit(uint a, int k)
        {
            // возврат k-го бита из числа a
            if (k > 31 || k < 0)
                throw new ArgumentException("k must be in range 0 to 31.");
            return (byte)((a >> k) & 1);
        }

        // Task12
        public static uint SwitchKthBit(uint a, int k, bool isSet)
        {
            // установить/снять k-й бит числа a
            if (k > 31 || k < 0)
                throw new ArgumentException("k must be in range 0 to 31.");
            if (isSet)
                return a | (1u << k);
            else
                return a & ~(1u << k);
        }

        // Task13
        public static uint SwapBits(uint a, int i, int j)
        {
            // поменять i-й и j-й биты местами
            if (i < 0 || i > 31 || j < 0 || j > 31)
                throw new ArgumentException("i and j must be in range 0 to 31.");
            
            uint tm = ((a >> i) ^ (a >> j)) & 1;
            if (tm != 0)
                return a ^ (tm << i) ^ (tm << j);
            else
                return a;
        }

        // Task14
        public static uint NullifyMLowBits(uint a, int m)
        {
            // обнуляет m младших бит числа a
            if (m < 0 || m > 32)
                throw new ArgumentException("m must be in range 0 to 32");
            return a >> m << m;
        }

        // Task21
        public static uint ConcatExtremeBits(uint a, int len, int i)
        {
            // склеивает первые i бит с последними
            if (len < 2 || len > 32)
                throw new ArgumentException("len must be in range 2 to 32.");
            if (i < 0 || i > 16 || i > len)
                throw new ArgumentException("i must be in range 0 to 16 and <= len.");

            return (a >> (len - i) << i) | (a & (1u << i) - 1);
        }

        // Task22
        public static uint GetCentralBits(uint a, int len, int i)
        {
            // получает биты, находящиеся между первыми и последними i битами
            if (len < 1 || len > 32)
                throw new ArgumentException("len must be in range 1 to 32");
            if (i < 0 || i > len / 2)
                throw new ArgumentException("i must be >= 0 and <= len / 2");

            return a >> i & ((1u << len - 2 * i) - 1);
        }

        // Task3
        public static uint BytePermutation(uint a, byte[] indices)
        {
            // поменять местами байты в соответствии с заданной перестановкой
            // indices[i] = 0   соответствует младшему байту исходного числа
            // indices[0]   соответствует старшему байту результата
            if (indices.Length != 4)
                throw new ArgumentException("Length of indices must be 4.");
            foreach (byte index in indices)
                if (index < 0 || index > 3)
                    throw new ArgumentException("Each index must be in range 0 to 3.");

            byte[] aBytes = BitConverter.GetBytes(a);
            byte[] result = new byte[4];
            for (int i = 0; i < 4; ++i)
                result[i] = aBytes[indices[3 - i]];

            return BitConverter.ToUInt32(result, 0);
        }

        // Task4
        public static uint Task4(uint a)
        {
            // максимальная степень 2, на которую делится число a
            return a & (~a + 1);
        }

        // Task5
        public static int Task5(uint x)
        {
            // находит показатель p степени 2 такой, что 2^p <= x <= 2^{p+1}
            if (x == 0)
                throw new ArgumentException("x must be > 0");

            int p = 0;
            while (x > 1)
            {
                ++p;
                x >>= 1;
            }
            return p;
        }

        // Task6
        public static byte XorBits(uint value, byte p)
        {
            // TODOL: универсализировать для длины в битах
            // xor всех бит числа a
            if (p > 5)
                p = 5;

            byte tm = (byte)(1 << (p - 1));
            while (tm > 0)
            {
                value ^= value >> tm;
                tm /= 2;
            }
            return (byte)(value & 1);
        }

        public static byte XorBits(byte value)
        {
            // xor всех бит числа a
            byte p = 3;

            byte tm = (byte)(1 << (p - 1));
            while (tm > 0)
            {
                value ^= (byte)(value >> tm);
                tm /= 2;
            }
            return (byte)(value & 1);
        }

        // Task7
        public static uint CycleShiftLeft(uint a, byte len, byte n)
        {
            n %= len;
            return ((a << n) | (a >> (len - n))) & ((1u << len) - 1);// последнее для обнуления высших разрядов
        }

        public static ulong CycleShiftLeft(ulong a, byte len, byte n)
        {
            n %= len;
            return ((a << n) | (a >> (len - n))) & ((1ul << len) - 1);
        }

        public static uint CycleShiftRight(uint a, byte len, byte n)
        {
            n %= len;
            return ((a >> n) | (a << len - n)) & ((1u << len) - 1);
        }

        public static ulong CycleShiftRight(ulong a, byte len, byte n)
        {
            n %= len;
            return ((a >> n) | (a << len - n)) & ((1ul << len) - 1);
        }

        // Task8
        public static ulong BitPermutation(ulong source, byte[] permutation)
        {
            if (permutation.Length > 64)
                throw new Exception("Wrong length of permutation array.");

            ulong result = 0;
            foreach (byte index in permutation)
            {
                if (index > 63)
                    throw new Exception("Wrong index in permutation.");
                result = (result << 1) | ((source >> index) & 1);
            }
            return result;
        }

        public static uint BitPermutation(uint source, byte[] permutation)
        {
            if (permutation.Length > 32)
                throw new Exception("Wrong length of permutation array.");

            uint result = 0;
            foreach (byte index in permutation)
            {
                if (index > 31)
                    throw new Exception("Wrong index in permutation.");
                result = (result << 1) | ((source >> index) & 1);
            }
            return result;
        }

    }
}
