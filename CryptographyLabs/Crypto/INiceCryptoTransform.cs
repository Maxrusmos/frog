using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.Crypto
{
    // TODOL NiceFinalTransform with bytesCount equals 0 (if text is empty)
    public interface INiceCryptoTransform
    {
        int InputBlockSize { get; }
        int OutputBlockSize { get; }

        /// <summary>
        /// as ICryptoTransform but with pleasure
        /// </summary>
        /// <param name="blocksCount">count of blocks (not bytes as in ICryptoTransform)</param>
        void NiceTransform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset, int blocksCount);

        /// <summary>
        /// as ICryptoTransform but without empty final block
        /// </summary>
        /// <param name="bytesCount">from 1 to InputBytesCount (including)</param>
        byte[] NiceFinalTransform(byte[] inputBuffer, int inputOffset, int bytesCount);
    }
}
