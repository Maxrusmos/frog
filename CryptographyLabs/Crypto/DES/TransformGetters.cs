using CryptographyLabs.Crypto.BlockCouplingModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.Crypto
{
    public static partial class DES_
    {
        public static ICryptoTransform Get(ulong key56, byte[] IV, Mode mode, CryptoDirection direction)
        {
            switch (mode)
            {
                default:
                case Mode.ECB:
                    return Get(key56, direction);
                case Mode.CBC:
                    return CBC.Get(GetNice(key56, direction), IV, direction);
                case Mode.CFB:
                    return CFB.Get(GetNice(key56, CryptoDirection.Encrypt), IV, direction);
                case Mode.OFB:
                    return OFB.Get(GetNice(key56, CryptoDirection.Encrypt), IV, direction);
            }
        }

        public static ICryptoTransform Get(ulong key56, CryptoDirection direction)
        {
            if (direction == CryptoDirection.Encrypt)
                return new DESEncryptTransform(key56);
            else
                return new DESDecryptTransform(key56);
        }

        public static INiceCryptoTransform GetNice(ulong key56, CryptoDirection direction)
        {
            if (direction == CryptoDirection.Encrypt)
                return new DESEncryptTransform(key56);
            else
                return new DESDecryptTransform(key56);
        }

    }
}
