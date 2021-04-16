using CryptographyLabs.Crypto.BlockCouplingModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.Crypto
{
    public static partial class Rijndael_
    {
        /// <exception cref="ArgumentException">Wrong key length or IV length</exception>
        public static ICryptoTransform Get(byte[] key, Size stateSize, byte[] IV, Mode mode, CryptoDirection direction)
        {
            switch (mode)
            {
                default:
                case Mode.ECB:
                    return Get(key, stateSize, direction);
                case Mode.CBC:
                    return CBC.Get(GetNice(key, stateSize, direction), IV, direction);
                case Mode.CFB:
                    return CFB.Get(GetNice(key, stateSize, CryptoDirection.Encrypt), IV, direction);
                case Mode.OFB:
                    return OFB.Get(GetNice(key, stateSize, CryptoDirection.Encrypt), IV, direction);
            }
        }

        /// <exception cref="ArgumentException">Wrong key length</exception>
        public static ICryptoTransform Get(byte[] key, Size stateSize, CryptoDirection direction)
        {
            if (!IsValidKeyLength(key))
                throw new ArgumentException("Wrong key length.");

            if (direction == CryptoDirection.Encrypt)
                return new RijndaelEncryptTransform(stateSize, key);
            else
                return new RijndaelDecryptTransform(stateSize, key);
        }

        /// <exception cref="ArgumentException">Wrong key length</exception>
        public static INiceCryptoTransform GetNice(byte[] key, Size stateSize, CryptoDirection direction)
        {
            if (!IsValidKeyLength(key))
                throw new ArgumentException("Wrong key length.");

            if (direction == CryptoDirection.Encrypt)
                return new RijndaelEncryptTransform(stateSize, key);
            else
                return new RijndaelDecryptTransform(stateSize, key);
        }

        private static bool IsValidKeyLength(byte[] key)
        {
            return key.Length == 16 || key.Length == 24 || key.Length == 32;
        }
    }
}
