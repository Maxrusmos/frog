using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.Crypto.BlockCouplingModes
{
    public static class CBC
    {
        /// <exception cref="ArgumentException">Wrong length of IV</exception>
        public static ICryptoTransform Get(INiceCryptoTransform transform, byte[] IV, CryptoDirection direction)
        {
            if (direction == CryptoDirection.Encrypt)
                return new CBCEncryptTransform(transform, IV);
            else
                return new CBCDecryptTransform(transform, IV);
        }
    }

    public class CBCEncryptTransform : BaseEncryptTransform
    {
        private byte[] _initVector;

        /// <exception cref="ArgumentException">Wrong length of IV</exception>
        public CBCEncryptTransform(INiceCryptoTransform transform, byte[] IV) : base(transform)
        {
            if (IV.Length != InputBlockSize)
                throw new ArgumentException("Wrong length of IV.");

            _initVector = new byte[InputBlockSize];
            Array.Copy(IV, _initVector, InputBlockSize);
        }

        #region BaseEncryptTransform

        protected override void Transform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
        {
            for (int i = 0; i < InputBlockSize; i++)
                inputBuffer[inputOffset + i] ^= _initVector[i];
            _baseTransform.NiceTransform(inputBuffer, inputOffset, outputBuffer, outputOffset, 1);
            Array.Copy(outputBuffer, outputOffset, _initVector, 0, InputBlockSize);
        }

        #endregion
    }

    public class CBCDecryptTransform : BaseDecryptTransform
    {
        private byte[] _initVector;

        /// <exception cref="ArgumentException">Wrong length of IV</exception>
        public CBCDecryptTransform(INiceCryptoTransform transform, byte[] IV) : base(transform)
        {
            if (IV.Length != InputBlockSize)
                throw new ArgumentException("Wrong length of IV.");

            _initVector = new byte[InputBlockSize];
            Array.Copy(IV, _initVector, InputBlockSize);
        }

        #region BaseDecryptTransform

        protected override void Transform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
        {
            _baseTransform.NiceTransform(inputBuffer, inputOffset, outputBuffer, outputOffset, 1);
            for (int i = 0; i < InputBlockSize; i++)
                outputBuffer[outputOffset + i] ^= _initVector[i];
            Array.Copy(inputBuffer, inputOffset, _initVector, 0, InputBlockSize);
        }

        #endregion
    }

}
