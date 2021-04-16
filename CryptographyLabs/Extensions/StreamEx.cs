using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptographyLabs
{
    public static class StreamEx
    {
        public static void CopyTo(this Stream from, Stream destination, int bufSize,
            CancellationToken token, Action<double> progressCallback = null)
        {
            progressCallback?.Invoke(0);
            byte[] buffer = new byte[bufSize];
            long totalWrote = 0;
            while (true)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                int hasRead = from.Read(buffer, 0, bufSize);
                if (hasRead == 0)
                    break;
                destination.Write(buffer, 0, hasRead);
                totalWrote += hasRead;
                progressCallback?.Invoke((double)totalWrote / from.Length);
            }
            progressCallback?.Invoke(1);
        }

        public static async Task CopyToAsync(this Stream from, Stream destination, int bufSize,
            CancellationToken token, Action<double> progressCallback = null)
        {
            progressCallback?.Invoke(0);
            byte[] buffer = new byte[bufSize];
            long totalWrote = 0;
            while (true)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                int hasRead = await from.ReadAsync(buffer, 0, bufSize);
                if (hasRead == 0)
                    break;
                await destination.WriteAsync(buffer, 0, hasRead);
                totalWrote += hasRead;
                progressCallback?.Invoke((double)totalWrote / from.Length);
            }
            progressCallback?.Invoke(1);
        }
    }
}
