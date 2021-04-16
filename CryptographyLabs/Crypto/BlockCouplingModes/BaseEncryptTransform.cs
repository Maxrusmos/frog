using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.Crypto.BlockCouplingModes
{
    public abstract class BaseEncryptTransform : ICryptoTransform
    {
        protected INiceCryptoTransform _baseTransform;
        private bool _hasBlocks = false;

        public BaseEncryptTransform(INiceCryptoTransform transform)
        {
            if (transform.InputBlockSize != transform.OutputBlockSize)
                throw new CryptographicException("InputBlockSize != OutputBlockSize.");

            _baseTransform = transform;
        }

        #region ICryptoTransform

        public int InputBlockSize => _baseTransform.InputBlockSize;
        public int OutputBlockSize => _baseTransform.OutputBlockSize;
        public bool CanTransformMultipleBlocks => true;
        public bool CanReuseTransform => false;

        public void Dispose()
        {

        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            int blocksCount = inputCount / InputBlockSize;
            if (blocksCount > 0 && !_hasBlocks)
                _hasBlocks = true;

            for (int i = 0; i < blocksCount; i++)
                Transform(inputBuffer, inputOffset + i * InputBlockSize,
                    outputBuffer, outputOffset + i * InputBlockSize);
            return blocksCount * InputBlockSize;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputCount == 0)
            {
                byte[] buf = new byte[InputBlockSize];
                if (_hasBlocks)
                    buf[0] = 8;
                else
                    buf[0] = 0;
                byte[] final = new byte[InputBlockSize];
                Transform(buf, 0, final, 0);
                return final;
            }
            else
            {
                byte[] final = new byte[2 * InputBlockSize];
                Transform(inputBuffer, inputOffset, final, 0);

                byte[] buf = new byte[InputBlockSize];
                buf[0] = (byte)inputCount;
                Transform(buf, 0, final, InputBlockSize);

                return final;
            }
        }

        protected abstract void Transform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset);

        #endregion

    }
}
