using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CryptographyLabs.Crypto
{
    public abstract class BlockDecryptTransform : INiceCryptoTransform, ICryptoTransform
    {
        private int _inputBlockSize;
        private int _outputBlockSize;
        private bool _isFirst = true;
        private byte[] _prevText;

        public BlockDecryptTransform(int blockSize) : this(blockSize, blockSize)
        { }

        public BlockDecryptTransform(int inputBlockSize, int outputBlockSize)
        {
            _inputBlockSize = inputBlockSize;
            _outputBlockSize = outputBlockSize;
            _prevText = new byte[InputBlockSize];
        }

        #region ICryptoTransform

        public int InputBlockSize => _inputBlockSize;
        public int OutputBlockSize => _outputBlockSize;
        public bool CanTransformMultipleBlocks => true;
        public bool CanReuseTransform => false;

        abstract public void Dispose();

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            int blocksCount = inputCount / InputBlockSize;
            if (_isFirst)
            {
                for (int i = 0; i < blocksCount - 1; i++)
                {
                    Transform(inputBuffer, inputOffset + i * InputBlockSize,
                        outputBuffer, outputOffset + i * OutputBlockSize);
                }
                Array.Copy(inputBuffer, inputOffset + (blocksCount - 1) * InputBlockSize,
                    _prevText, 0, InputBlockSize);
                _isFirst = false;
                return (blocksCount - 1) * OutputBlockSize;
            }
            else
            {
                Transform(_prevText, 0, outputBuffer, outputOffset);
                for (int i = 0; i < blocksCount - 1; i++)
                {
                    Transform(inputBuffer, inputOffset + i * InputBlockSize,
                        outputBuffer, outputOffset + (i + 1) * OutputBlockSize);
                }
                Array.Copy(inputBuffer, inputOffset + (blocksCount - 1) * InputBlockSize,
                    _prevText, 0, InputBlockSize);
                return blocksCount * OutputBlockSize;
            }
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputCount != 0)
                throw new CryptographicException("Wrong length of final block on decryption.");

            if (_isFirst)
                throw new CryptographicException("Not found final block on decryption.");

            byte[] final = new byte[OutputBlockSize];
            Transform(_prevText, 0, final, 0);
            byte finalBlockSize = final[OutputBlockSize - 1];
            if (finalBlockSize >= OutputBlockSize)
                throw new CryptographicException("Final block corrupted.");
            Array.Resize(ref final, finalBlockSize);
            return final;
        }

        #endregion

        #region INiceCryptoTransform

        public void NiceTransform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset, int blocksCount)
        {
            for (int i = 0; i < blocksCount; i++)
            {
                Transform(inputBuffer, inputOffset + i * InputBlockSize,
                    outputBuffer, outputOffset + i * OutputBlockSize);
            }
        }

        public byte[] NiceFinalTransform(byte[] inputBuffer, int inputOffset, int bytesCount)
        {
            if (bytesCount != InputBlockSize)
                throw new CryptographicException("Wrong length of final block on decryption.");

            byte[] final = new byte[OutputBlockSize];
            Transform(inputBuffer, inputOffset, final, 0);
            byte finalBlockSize = final[OutputBlockSize - 1];
            if (finalBlockSize >= OutputBlockSize)
                throw new CryptographicException("Final block corrupted.");
            Array.Resize(ref final, finalBlockSize);
            return final;
        }

        #endregion

        protected abstract void Transform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset);

    }
}
