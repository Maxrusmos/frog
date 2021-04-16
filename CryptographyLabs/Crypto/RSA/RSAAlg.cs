using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CryptographyLabs.Crypto
{
    public static class RSAAlg
    {
        // Task 5
        private static int BinPowMod(int a, int pow, int mod)
        {
            if (a == 0 && pow == 0)
                throw new ArgumentException("a = 0 and pow = 0. That's all.");
            if (mod < 1)
                throw new ArgumentException("mod must be > 0.");

            a %= mod;
            if (a < 0)
                a += mod;

            if (pow < 0)
            {
                if (GCD(a, mod) != 1)
                    throw new Exception("Can't find inverse value.");
                pow %= EulerFunc(mod);
                pow += mod;
            }

            int res = 1;
            while (pow > 0)
            {
                if ((pow & 1) == 1)
                    res = (res * a) % mod;
                a = (a * a) % mod;
                pow >>= 1;
            }
            return res;
        }

        public static BigInteger BinPowMod(BigInteger a, BigInteger pow, BigInteger mod)
        {
            if (a == 0 && pow == 0)
                throw new ArgumentException("a = 0 and pow = 0. That's all.");
            if (pow < 0)
                throw new ArgumentException("Pow must be >= 0.");
            if (mod < 1)
                throw new ArgumentException("mod must be > 0.");

            a %= mod;
            if (a < 0)
                a += mod;

            BigInteger result = 1;
            while (pow > 0)
            {
                if ((pow & 1) == 1)
                    result = (result * a) % mod;
                a = (a * a) % mod;
                pow >>= 1;
            }
            return result;
        }

        // Task 2
        public static List<ulong> ReducedDeductionSystem(ulong mod)
        {
            if (mod < 2)
                throw new ArgumentException("Mod must be >= 2.");

            List<ulong> result = new List<ulong>();
            result.Add(1ul);

            for (ulong i = 2; i < mod; ++i)
                if (GCD(i, mod) == 1ul)
                    result.Add(i);
            return result;
        }

        // Task 6
        private static int GCD(int a, int b)
        {
            if (a <= 0 || b <= 0)
                throw new ArgumentException("Values must be > 0.");

            while (b > 0)
            {
                int tm = a;
                a = b;
                b = tm % b;
            }
            return a;
        }

        private static ulong GCD(ulong a, ulong b)
        {
            if (a == 0 || b == 0)
                throw new ArgumentException("Values must be > 0.");

            while (b > 0)
            {
                ulong tm = a;
                a = b;
                b = tm % b;
            }
            return a;
        }

        public static BigInteger GCD(BigInteger a, BigInteger b)
        {
            if (a <= 0 || b <= 0)
                throw new ArgumentException("Values must be > 0.");

            while (b > 0)
            {
                BigInteger tm = a;
                a = b;
                b = tm % b;
            }
            return a;
        }

        // Task 7
        private static int GCD(int a, int b, out int x, out int y)
        {
            if (a <= 0 || b <= 0)
                throw new ArgumentException("Value must be > 0.");

            x = 1;
            y = 0;
            int xx = 0;
            int yy = 1;
            while (b > 0)
            {
                int q = a / b;
                int tm = a;
                a = b;
                b = tm % b;

                tm = x;
                x = xx;
                xx = tm - xx * q;

                tm = y;
                y = yy;
                yy = tm - yy * q;
            }
            return a;
        }

        public static BigInteger GCD(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y)
        {
            if (a <= 0 || b <= 0)
                throw new ArgumentException("Value must be > 0.");

            x = 1;
            y = 0;
            BigInteger xx = 0;
            BigInteger yy = 1;
            while (b > 0)
            {
                BigInteger q = a / b;
                BigInteger tm = a;
                a = b;
                b = tm % b;

                tm = x;
                x = xx;
                xx = tm - xx * q;

                tm = y;
                y = yy;
                yy = tm - yy * q;
            }
            return a;
        }

        // Task 3
        /// <summary>
        /// bad realisation
        /// </summary>
        public static int EulerFunc_(int m)
        {
            if (m < 2)
                throw new ArgumentException("m must be >= 2.");

            int res = 0;
            for (int i = 1; i < m; ++i)
                if (GCD(i, m) == 1)
                    res++;
            return res;
        }

        /// <summary>
        /// normal realisation with factor out
        /// </summary>
        /// <exception cref="ArgumentException">m < 2</exception>
        public static int EulerFunc(int m)
        {
            if (m < 2)
                throw new ArgumentException("m must be >= 2.");

            FactorOut(m, out List<int> primes, out List<int> degrees);
            int result = 1;
            for (int i = 0; i < primes.Count; i++)
            {
                int tm = (int)Math.Pow(primes[i], degrees[i] - 1);
                result *= primes[i] * tm - tm;
            }
            return result;
        }

        // Task 4
        public static void FactorOut(int value, out List<int> primes, out List<int> degrees)
        {
            if (value < 1)
                throw new ArgumentException("value must be >= 1.");
            else if (value < 3)
            {
                primes = new List<int> { value };
                degrees = new List<int> { 1 };
                return;
            }

            primes = CalcPrimes((int)Math.Sqrt(value) + 1);
            degrees = new List<int>(primes.Count);
            for (int i = 0; i < primes.Count; ++i) degrees.Add(0);

            for (int i = 0; i < primes.Count; ++i)
            {
                while (value % primes[i] == 0)
                {
                    value /= primes[i];
                    degrees[i]++;
                }
                if (value == 1)
                    break;
            }
            if (value > 1)
            {
                primes.Add(value);
                degrees.Add(1);
            }

            for (int i = primes.Count - 1; i > -1; --i)
            {
                if (degrees[i] == 0)
                {
                    primes.RemoveAt(i);
                    degrees.RemoveAt(i);
                }
            }
        }

        // Task 1
        /// <summary>
        /// Primary numbers from 2 to m (exclude m)
        /// </summary>
        /// <param name="m">m > 2</param>
        /// <returns>List of primes</returns>
        /// <exception cref="ArgumentException">m <= 2</exception>
        public static List<int> CalcPrimes(int m)
        {
            if (m <= 2)
                throw new ArgumentException("m must be > 2.");

            BitArray bitArray = new BitArray(m, true);
            for (int i = 2, i1 = (int)Math.Sqrt(m); i <= i1; ++i)
            {
                if (bitArray[i])
                {
                    for (int j = i * i; j < m; j += i)
                        bitArray[j] = false;
                }
            }

            List<int> result = new List<int>();
            for (int i = 2; i < m; ++i)
                if (bitArray[i])
                    result.Add(i);
            return result;
        }
    }
}
