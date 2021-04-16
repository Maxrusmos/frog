using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CryptographyLabs.Crypto
{
    public partial class FROGProvider
    {
        private class FROGDecryptTransform : BlockDecryptTransform
        {
            private byte[][][] _roundKeys;

            public FROGDecryptTransform(byte[][][] roundKeys) : base(BlockSize)
            {
                _roundKeys = roundKeys;
            }

            #region BlockDecryptTransform

            public override void Dispose()
            {

            }

            protected override void Transform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
            {
                Array.Copy(inputBuffer, inputOffset, outputBuffer, outputOffset, InputBlockSize);
                for (int round = 7; round >= 0; round--)
                {
                    for (int i = InputBlockSize - 1; i >= 0; i--)
                    {
                        // 4
                        byte index = _roundKeys[round][2][i];
                        outputBuffer[outputOffset + index] ^= outputBuffer[outputOffset + i];
                        // 3
                        if (i < InputBlockSize - 1)
                            outputBuffer[outputOffset + i + 1] ^= outputBuffer[outputOffset + i];
                        // 2
                        outputBuffer[outputOffset + i] = _roundKeys[round][1][outputBuffer[outputOffset + i]];
                        // 1
                        outputBuffer[outputOffset + i] ^= _roundKeys[round][0][i];
                    }
                }
            }

            #endregion
        }

    }
}
