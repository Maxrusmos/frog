using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.Crypto.BlockCouplingModes
{
    public static class CFB
    {
        /// <exception cref="ArgumentException">Wrong length of IV</exception>
        public static ICryptoTransform Get(INiceCryptoTransform transform, byte[] IV, CryptoDirection direction)
        {
            if (direction == CryptoDirection.Encrypt)
                return new CFBEncryptTransform(transform, IV);
            else
                return new CFBDecryptTransform(transform, IV);
        }

        private class CFBEncryptTransform : BaseEncryptTransform
        {
            private byte[] _initVector;

            /// <exception cref="ArgumentException">Wrong length of IV</exception>
            public CFBEncryptTransform(INiceCryptoTransform transform, byte[] IV) : base(transform)
            {
                if (IV.Length != InputBlockSize)
                    throw new ArgumentException("Wrong length of IV.");

                _initVector = new byte[InputBlockSize];
                Array.Copy(IV, _initVector, InputBlockSize);
            }

            #region BaseEncryptTransform

            protected override void Transform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
            {
                _baseTransform.NiceTransform(_initVector, 0, outputBuffer, outputOffset, 1);
                for (int i = 0; i < InputBlockSize; i++)
                    outputBuffer[outputOffset + i] ^= inputBuffer[inputOffset + i];
                Array.Copy(outputBuffer, outputOffset, _initVector, 0, InputBlockSize);
            }

            #endregion
        }

        private class CFBDecryptTransform : BaseDecryptTransform
        {
            private byte[] _initVector;

            /// <exception cref="ArgumentException">Wrong length of IV</exception>
            public CFBDecryptTransform(INiceCryptoTransform transform, byte[] IV) : base(transform)
            {
                if (IV.Length != InputBlockSize)
                    throw new ArgumentException("Wrong length of IV.");

                _initVector = new byte[InputBlockSize];
                Array.Copy(IV, _initVector, InputBlockSize);
            }

            #region BaseEncryptTransform

            protected override void Transform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
            {
                _baseTransform.NiceTransform(_initVector, 0, outputBuffer, outputOffset, 1);
                Array.Copy(inputBuffer, inputOffset, _initVector, 0, InputBlockSize);
                for (int i = 0; i < InputBlockSize; i++)
                    outputBuffer[outputOffset + i] ^= inputBuffer[inputOffset + i];
            }

            #endregion

        }
    }

    

    

}
