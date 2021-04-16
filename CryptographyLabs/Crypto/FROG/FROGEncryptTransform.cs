using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CryptographyLabs.Crypto
{
    public partial class FROGProvider
    {
        private class FROGEncryptTransform : BlockEncryptTransform
        {
            private byte[][][] _roundKeys;

            public FROGEncryptTransform(byte[][][] roundKeys) : base(BlockSize)
            {
                _roundKeys = roundKeys;
            }

            #region BlockEncryptTransform

            public override void Dispose()
            {

            }
            
            protected override void Transform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
            {
                Array.Copy(inputBuffer, inputOffset, outputBuffer, outputOffset, InputBlockSize);
                for (int round = 0; round < 8; round++)
                {
                    for (int i = 0; i < InputBlockSize; i++)
                    {
                        // 1
                        outputBuffer[outputOffset + i] ^= _roundKeys[round][0][i];
                        // 2
                        outputBuffer[outputOffset + i] = _roundKeys[round][1][outputBuffer[outputOffset + i]];
                        // 3
                        if (i < InputBlockSize - 1)
                            outputBuffer[outputOffset + i + 1] ^= outputBuffer[outputOffset + i];
                        // 4
                        byte index = _roundKeys[round][2][i];
                        outputBuffer[outputOffset + index] ^= outputBuffer[outputOffset + i];
                    }
                }
            }

            #endregion

        }

    }
}
