using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.Crypto.BlockCouplingModes
{
    public abstract class BaseDecryptTransform : ICryptoTransform
    {
        protected INiceCryptoTransform _baseTransform;
        private byte _prevTextsCount = 0;
        private byte[] _prevText;

        public BaseDecryptTransform(INiceCryptoTransform transform)
        {
            if (transform.InputBlockSize != transform.OutputBlockSize)
                throw new CryptographicException("InputBlockSize != OutputBlockSize.");

            _baseTransform = transform;
            _prevText = new byte[2 * InputBlockSize];
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

            if (blocksCount == 0)
                return 0;
            else if (blocksCount == 1)
            {
                if (_prevTextsCount == 2)
                {
                    Transform(_prevText, 0, outputBuffer, outputOffset);
                    Array.Copy(_prevText, InputBlockSize, _prevText, 0, InputBlockSize);
                    Array.Copy(inputBuffer, inputOffset, _prevText, InputBlockSize, InputBlockSize);
                    return InputBlockSize;
                }
                else
                {
                    Array.Copy(inputBuffer, inputOffset, _prevText,
                        _prevTextsCount * InputBlockSize, InputBlockSize);
                    _prevTextsCount++;
                    return 0;
                }
            }
            else// >= 2
            {
                for (int i = 0; i < _prevTextsCount; i++)
                    Transform(_prevText, i * InputBlockSize,
                        outputBuffer, outputOffset + i * InputBlockSize);

                for (int i = 0; i < blocksCount - 2; i++)
                    Transform(inputBuffer, inputOffset + i * InputBlockSize,
                        outputBuffer, (i + _prevTextsCount) * InputBlockSize);

                Array.Copy(inputBuffer, inputOffset + (blocksCount - 2) * InputBlockSize,
                    _prevText, 0, InputBlockSize);
                Array.Copy(inputBuffer, inputOffset + (blocksCount - 1) * InputBlockSize,
                    _prevText, InputBlockSize, InputBlockSize);

                int blocksTransformed = blocksCount - 2 + _prevTextsCount;
                _prevTextsCount = 2;
                return blocksTransformed * InputBlockSize;
            }
        }

        // задерживать 2 блока необходимо, т.к. в последнем хранится 
        // только количество действительных байт предпоследнего блока
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if(inputCount != 0)
                throw new CryptographicException("Wrong length of final block on OFB decryption.");

            if (_prevTextsCount == 0)
                throw new CryptographicException("Wrong count of blocks on OFB decryption.");
            else if (_prevTextsCount == 1)
            {
                byte[] buf = new byte[InputBlockSize];
                Transform(_prevText, 0, buf, 0);
                if (buf[0] != 0)
                    throw new CryptographicException("Final block is broken.");
                return Array.Empty<byte>();
            }
            else
            {
                byte[] buf = new byte[2 * InputBlockSize];
                Transform(_prevText, 0, buf, 0);
                Transform(_prevText, InputBlockSize, buf, InputBlockSize);
                if (buf[InputBlockSize] > InputBlockSize)
                    throw new CryptographicException("Final block is broken.");
                Array.Resize(ref buf, buf[InputBlockSize]);
                return buf;
            }
        }

        protected abstract void Transform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset);

        #endregion
    }
}
