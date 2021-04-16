using CryptographyLabs.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptographyLabs.GUI
{
    class DESEncryptTransformVM : BaseTransformVM
    {
        public DESEncryptTransformVM(string filePath, string encryptFilePath, ulong key56, byte[] IV, 
            DES_.Mode mode, bool isDeleteAfter) : base(isDeleteAfter, CryptoDirection.Encrypt)
        {
            CryptoName = "DES";
            SourceFilePath = filePath;
            DestFilePath = encryptFilePath;

            Start(DES_.Get(key56, IV, mode, CryptoDirection.Encrypt));
        }

        public DESEncryptTransformVM(string filePath, string encryptFilePath, ulong key56, bool isDeleteAfter,
            bool multithread = false) 
            : base(isDeleteAfter, CryptoDirection.Encrypt)
        {
            CryptoName = "DES";
            SourceFilePath = filePath;
            DestFilePath = encryptFilePath;

            if (multithread)
            {
                StartMultithread(DES_.GetNice(key56, CryptoDirection.Encrypt));
            }
            else
            {
                Start(DES_.Get(key56, CryptoDirection.Encrypt));
            }
        }
    }
}
