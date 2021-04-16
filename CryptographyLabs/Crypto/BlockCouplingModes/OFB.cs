using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.Crypto.BlockCouplingModes
{

    public static class OFB
    {
        /// <exception cref="ArgumentException">Wrong length of IV</exception>
        public static ICryptoTransform Get(INiceCryptoTransform transform, byte[] IV, CryptoDirection direction)
        {
            if (direction == CryptoDirection.Encrypt)
                return new OFBEncryptTransform(transform, IV);
            else
                return new OFBDecryptTransform(transform, IV);
        }
    }

    public class OFBEncryptTransform : BaseEncryptTransform
    {
        private byte[] _initVector;

        /// <exception cref="ArgumentException">Wrong length of IV</exception>
        public OFBEncryptTransform(INiceCryptoTransform transform, byte[] IV) : base(transform)
        {
            if (IV.Length != InputBlockSize)
                throw new ArgumentException("Wrong length of IV.");

            _initVector = new byte[InputBlockSize];
            Array.Copy(IV, _initVector, InputBlockSize);
        }

        #region BaseDecryptTransform

        protected override void Transform(byte[] inputBuffer, int inputOffset,byte[] outputBuffer, int outputOffset)
        {
            _baseTransform.NiceTransform(_initVector, 0, outputBuffer, outputOffset, 1);
            Array.Copy(outputBuffer, outputOffset, _initVector, 0, InputBlockSize);
            for (int i = 0; i < InputBlockSize; i++)
                outputBuffer[outputOffset + i] ^= inputBuffer[inputOffset + i];
        }

        #endregion

    }

    public class OFBDecryptTransform : BaseDecryptTransform
    {
        private byte[] _initVector;

        /// <exception cref="ArgumentException">Wrong length of IV</exception>
        public OFBDecryptTransform(INiceCryptoTransform transform, byte[] IV) : base(transform)
        {
            if (IV.Length != InputBlockSize)
                throw new ArgumentException("Wrong length of IV.");

            _initVector = new byte[InputBlockSize];
            Array.Copy(IV, _initVector, InputBlockSize);
        }

        #region BaseDecryptTransform

        protected override void Transform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
        {
            _baseTransform.NiceTransform(_initVector, 0, outputBuffer, outputOffset, 1);
            Array.Copy(outputBuffer, outputOffset, _initVector, 0, InputBlockSize);
            for (int i = 0; i < InputBlockSize; i++)
                outputBuffer[outputOffset + i] ^= inputBuffer[inputOffset + i];
        }

        #endregion
    }

}
