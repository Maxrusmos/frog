using System;
using System.Collections.Generic;
using System.Text;
using CryptographyLabs.Crypto;

namespace CryptographyLabs.GUI
{
    public class FrogDecryptTransformVM : BaseTransformVM
    {
        public FrogDecryptTransformVM(string filePath, string decryptFilePath, byte[] key,
            bool isDeleteAfter, bool multithread = false) : base(isDeleteAfter, CryptoDirection.Decrypt)
        {
            CryptoName = "FROG";
            SourceFilePath = filePath;
            DestFilePath = decryptFilePath;

            FROGProvider provider = new FROGProvider(key);
            if (multithread)
                StartMultithread(provider.CreateNice(CryptoDirection.Decrypt));
            else
                Start(provider.Create(CryptoDirection.Decrypt));
        }

        public FrogDecryptTransformVM(string filePath, string decryptFilePath, byte[] key,
            FROGProvider.Mode mode, byte[] iv, bool isDeleteAfter) : base(isDeleteAfter, CryptoDirection.Decrypt)
        {
            CryptoName = "FROG";
            SourceFilePath = filePath;
            DestFilePath = decryptFilePath;

            FROGProvider provider = new FROGProvider(key);
            Start(provider.Create(CryptoDirection.Decrypt, mode, iv));
        }
    }
}
