using CryptographyLabs.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptographyLabs.GUI
{
    public class FrogEncryptTransformVM : BaseTransformVM
    {
        public FrogEncryptTransformVM(string filePath, string encryptFilePath, byte[] key, 
            bool isDeleteAfter, bool multithread = false) : base(isDeleteAfter, CryptoDirection.Encrypt)
        {
            CryptoName = "FROG";
            SourceFilePath = filePath;
            DestFilePath = encryptFilePath;

            FROGProvider provider = new FROGProvider(key);
            if (multithread)
                StartMultithread(provider.CreateNice(CryptoDirection.Encrypt));
            else
                Start(provider.Create(CryptoDirection.Encrypt));
        }

        public FrogEncryptTransformVM(string filePath, string encryptFilePath, byte[] key,
            FROGProvider.Mode mode, byte[] iv, bool isDeleteAfter) : base(isDeleteAfter, CryptoDirection.Encrypt)
        {
            CryptoName = "FROG";
            SourceFilePath = filePath;
            DestFilePath = encryptFilePath;

            FROGProvider provider = new FROGProvider(key);
            Start(provider.Create(CryptoDirection.Encrypt, mode, iv));

        }
    }
}
