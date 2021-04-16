using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using CryptographyLabs;

namespace CryptographyLabs.Crypto
{
    public static class RC4
    {
        public static ICryptoTransform Get(byte[] keyBytes)
        {
            return new RC4CryptoTransform(keyBytes);
        }

        private class RC4CryptoTransform : ICryptoTransform
        {
            public int InputBlockSize => 1;
            public int OutputBlockSize => 1;
            public bool CanTransformMultipleBlocks => true;
            public bool CanReuseTransform => false;

            private byte[] _SBlock = new byte[256];
            private byte index1 = 0, index2 = 0;

            public RC4CryptoTransform(byte[] key)
            {
                if (key.Length < 1 || key.Length > 256)
                    throw new ArgumentException("Key length must be in range [1; 256].");

                InitSBlock(key);
            }

            private void InitSBlock(byte[] key)
            {
                for (int i = 0; i < _SBlock.Length; ++i)
                    _SBlock[i] = (byte)i;

                byte j = 0;
                for (int i = 0; i < 256; ++i)
                {
                    j = (byte)(j + _SBlock[i] + key[i % key.Length]);
                    byte tm = _SBlock[i];
                    _SBlock[i] = _SBlock[j];
                    _SBlock[j] = tm;
                }
            }

            public void Dispose()
            {

            }

            public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
            {
                for (int i = 0; i < inputCount; ++i)
                {
                    ++index1;
                    index2 += _SBlock[index1];
                    byte tm = _SBlock[index1];
                    _SBlock[index1] = _SBlock[index2];
                    _SBlock[index2] = tm;
                    byte resIndex = (byte)(_SBlock[index1] + _SBlock[index2]);
                    outputBuffer[outputOffset + i] = (byte)(_SBlock[resIndex] ^ inputBuffer[inputOffset + i]);
                }
                return inputCount;
            }

            public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
            {
                byte[] result = new byte[inputCount];
                TransformBlock(inputBuffer, inputOffset, inputCount, result, 0);
                return result;
            }
        }
    }


}
