using CryptographyLabs.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptographyLabs.Crypto.BlockCouplingModes
{
    public static class ECB
    {
        /// <exception cref="ArgumentException">data is empty.</exception>
        /// <exception cref="OperationCanceledException">canceled</exception>
        public static async Task<byte[]> TransformAsync(byte[] data, INiceCryptoTransform transform, 
            CancellationToken token, int threadsCount = 4, Action<double> progressCallback = null)
        {
            if (data.Length == 0)
                throw new ArgumentException("Length of text if empty.");

            int blocksCount = data.Length / transform.InputBlockSize;
            int lastBlockSize = data.Length % transform.InputBlockSize;
            if (lastBlockSize == 0)
            {
                blocksCount--;
                lastBlockSize = transform.InputBlockSize;
            }

            byte[] result = new byte[blocksCount * transform.OutputBlockSize];

            int blocksPerThread = blocksCount / threadsCount;
            Task[] transformTasks = new Task[threadsCount];
            double[] progresses = new double[threadsCount];
            for (int i = 0; i < threadsCount; i++)
            {
                int currentBlocksCount = i == threadsCount - 1
                    ? blocksPerThread + blocksCount % threadsCount
                    : blocksPerThread;

                int i_ = i;
                transformTasks[i] = MakeTransformTask(transform, data, i * blocksPerThread * transform.InputBlockSize,
                    result, i * blocksPerThread * transform.OutputBlockSize, currentBlocksCount, token,
                    (progress) =>
                    {
                        progresses[i_] = progress;
                        progressCallback?.Invoke(MathEx.Sum(progresses) / threadsCount);
                    });
            }

            Task<byte[]> finalTask = Task.Run(() =>
            {
                byte[] buf = new byte[transform.InputBlockSize];
                Array.Copy(data, blocksCount * transform.InputBlockSize, buf, 0, lastBlockSize);
                return transform.NiceFinalTransform(buf, 0, lastBlockSize);
            });

            await Task.WhenAll(transformTasks);
            byte[] final = await finalTask;

            Array.Resize(ref result, result.Length + final.Length);
            Array.Copy(final, 0, result, blocksCount * transform.OutputBlockSize, final.Length);
            return result;
        }

        /// <summary>
        /// wrap for argumets
        /// </summary>
        /// <exception cref="OperationCanceledException">Task with this exception</exception>
        private static Task MakeTransformTask(INiceCryptoTransform transform, byte[] inBuf, int inOffset,
            byte[] outBuf, int outOffset, int blocksCount, CancellationToken token, Action<double> progressCallback = null)
        {
            return Task.Run(() =>
            {
                transform.NiceTransform(inBuf, inOffset, outBuf, outOffset, blocksCount, token, progressCallback);
            });
        }





        /// <exception cref="ArgumentException">data is empty.</exception>
        public static async Task<byte[]> TransformAsync(byte[] data, INiceCryptoTransform transform,
            int threadsCount = 4, Action<double> progressCallback = null)
        {
            if (data.Length == 0)
                throw new ArgumentException("Length of text if empty.");

            int blocksCount = data.Length / transform.InputBlockSize;
            int lastBlockSize = data.Length % transform.InputBlockSize;
            if (lastBlockSize == 0)
            {
                blocksCount--;
                lastBlockSize = transform.InputBlockSize;
            }

            byte[] result = new byte[blocksCount * transform.OutputBlockSize];

            int blocksPerThread = blocksCount / threadsCount;
            Task[] transformTasks = new Task[threadsCount];
            double[] progresses = new double[threadsCount];
            for (int i = 0; i < threadsCount; i++)
            {
                int currentBlocksCount = i == threadsCount - 1
                    ? blocksPerThread + blocksCount % threadsCount
                    : blocksPerThread;

                int i_ = i;
                transformTasks[i] = MakeTransformTask(transform, data, i * blocksPerThread * transform.InputBlockSize,
                    result, i * blocksPerThread * transform.OutputBlockSize, currentBlocksCount,
                    (progress) =>
                    {
                        progresses[i_] = progress;
                        progressCallback?.Invoke(MathEx.Sum(progresses) / threadsCount);
                    });
            }

            Task<byte[]> finalTask = Task.Run(() =>
            {
                byte[] buf = new byte[transform.InputBlockSize];
                Array.Copy(data, blocksCount * transform.InputBlockSize, buf, 0, lastBlockSize);
                return transform.NiceFinalTransform(buf, 0, lastBlockSize);
            });

            await Task.WhenAll(transformTasks);
            byte[] final = await finalTask;

            Array.Resize(ref result, result.Length + final.Length);
            Array.Copy(final, 0, result, blocksCount * transform.OutputBlockSize, final.Length);
            return result;
        }

        private static Task MakeTransformTask(INiceCryptoTransform transform, byte[] inBuf, int inOffset, 
            byte[] outBuf, int outOffset, int blocksCount, Action<double> progressCallback = null)
        {
            return Task.Run(() =>
            {
                transform.NiceTransform(inBuf, inOffset, outBuf, outOffset, blocksCount, progressCallback);
            });
        }

    }
}
