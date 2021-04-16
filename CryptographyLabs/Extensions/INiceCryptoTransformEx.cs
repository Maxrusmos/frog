using CryptographyLabs.Crypto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CryptographyLabs.Extensions
{
    public static class INiceCryptoTransformEx
    {
        public static void NiceTransform(this INiceCryptoTransform transform, byte[] inputBuffer, int inputOffset,
            byte[] outputBuffer, int outputOffset, int blocksCount, Action<double> progressCallback = null)
        {
            for (int i = 0; i < blocksCount; i++)
            {
                transform.NiceTransform(inputBuffer, inputOffset + i * transform.InputBlockSize,
                    outputBuffer, outputOffset + i * transform.OutputBlockSize, 1);
                progressCallback?.Invoke((double)(i + 1) / blocksCount);
            }
        }

        /// <summary>
        /// transform block by block with progress and cancellation token
        /// </summary>
        /// <exception cref="OperationCanceledException">canceled</exception>
        public static void NiceTransform(this INiceCryptoTransform transform, byte[] inputBuffer, int inputOffset,
            byte[] outputBuffer, int outputOffset, int blocksCount, CancellationToken token, 
            Action<double> progressCallback = null)
        {
            for (int i = 0; i < blocksCount; i++)
            {
                token.ThrowIfCancellationRequested();
                transform.NiceTransform(inputBuffer, inputOffset + i * transform.InputBlockSize,
                    outputBuffer, outputOffset + i * transform.OutputBlockSize, 1);
                progressCallback?.Invoke((double)(i + 1) / blocksCount);
            }
        }
    }
}
