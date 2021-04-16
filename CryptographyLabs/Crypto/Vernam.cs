using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using CryptographyLabs;

namespace CryptographyLabs.Crypto
{
    public static class Vernam
    {
        public static ICryptoTransform Get(Stream keyStream)
        {
            return new VernamCryptoTransform(keyStream);
        }

        private class VernamCryptoTransform : ICryptoTransform
        {
            public int InputBlockSize => 1;
            public int OutputBlockSize => 1;
            public bool CanTransformMultipleBlocks => true;
            public bool CanReuseTransform => false;

            private BufferedStream _keyStream;

            public VernamCryptoTransform(Stream keyStream)
            {
                _keyStream = new BufferedStream(keyStream);
            }

            public void Dispose()
            {
                _keyStream.Dispose();
            }

            public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
            {
                byte[] keyPart = new byte[inputCount];
                int hasRead = _keyStream.Read(keyPart, 0, inputCount);
                if (hasRead < inputCount)
                    throw new CryptographicException("Wrong length of key file on Vernam transform.");

                for (int i = 0; i < inputCount; ++i)
                {
                    outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ keyPart[i]);
                }
                return inputCount;
            }

            public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
            {
                byte[] result = new byte[inputCount];
                TransformBlock(inputBuffer, inputOffset, inputCount, result, 0);
                return result;
            }
        }

        public static async Task GenerateKeyFileAsync(string path, long bytesCount, int bufSize, 
            Action<double> progressCallback = null)
        {
            using (FileStream outStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                Random random = new Random((int)DateTime.Now.Ticks);
                byte[] buff = new byte[bufSize];
                progressCallback?.Invoke(0);
                for (long i = 0; i < bytesCount;)
                {
                    random.NextBytes(buff);
                    int toWrite = (int)Math.Min(bufSize, bytesCount - i);
                    await outStream.WriteAsync(buff, 0, toWrite);
                    i += toWrite;
                    progressCallback?.Invoke((double)i / bytesCount);
                }
                progressCallback?.Invoke(1);
            }
        }

    }


}
