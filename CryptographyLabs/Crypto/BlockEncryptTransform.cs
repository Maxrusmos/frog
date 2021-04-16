using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CryptographyLabs.Crypto
{
    public abstract class BlockEncryptTransform : INiceCryptoTransform, ICryptoTransform
    {
        private int _intputBlockSize;
        private int _outputBlockSize;

        public BlockEncryptTransform(int blockSize) : this(blockSize, blockSize)
        {

        }

        public BlockEncryptTransform(int intputBlockSize, int outputBlockSize)
        {
            _intputBlockSize = intputBlockSize;
            _outputBlockSize = outputBlockSize;
        }

        #region ICryptoTransform

        public int InputBlockSize => _intputBlockSize;
        public int OutputBlockSize => _outputBlockSize;
        public bool CanTransformMultipleBlocks => true;
        public bool CanReuseTransform => false;

        abstract public void Dispose();

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            int blocksCount = inputCount / InputBlockSize;
            NiceTransform(inputBuffer, inputOffset, outputBuffer, outputOffset, blocksCount);
            return blocksCount * OutputBlockSize;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            inputBuffer[InputBlockSize - 1] = (byte)inputCount;
            byte[] final = new byte[OutputBlockSize];
            Transform(inputBuffer, inputOffset, final, 0);
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
            if (bytesCount == InputBlockSize)
            {
                byte[] tm = new byte[InputBlockSize];
                tm[InputBlockSize - 1] = 0;

                byte[] final = new byte[2 * OutputBlockSize];
                Transform(inputBuffer, inputOffset, final, 0);
                Transform(tm, 0, final, OutputBlockSize);
                return final;
            }
            else
            {
                inputBuffer[InputBlockSize - 1] = (byte)bytesCount;
                byte[] final = new byte[OutputBlockSize];
                Transform(inputBuffer, inputOffset, final, 0);
                return final;
            }
        }

        #endregion

        protected abstract void Transform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset);

    }
}
