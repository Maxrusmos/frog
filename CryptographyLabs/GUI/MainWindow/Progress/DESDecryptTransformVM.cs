using CryptographyLabs.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.GUI
{
    class DESDecryptTransformVM : BaseTransformVM
    {
        public DESDecryptTransformVM(string filePath, string decryptFilePath, ulong key56, byte[] IV, DES_.Mode mode,
            bool isDeleteAfter) : base(isDeleteAfter, CryptoDirection.Decrypt)
        {
            CryptoName = "DES";
            SourceFilePath = filePath;
            DestFilePath = decryptFilePath;

            Start(DES_.Get(key56, IV, mode, CryptoDirection.Decrypt));
        }

        public DESDecryptTransformVM(string filePath, string decryptFilePath, ulong key56, bool isDeleteAfter,
            bool multithread = false) 
            : base(isDeleteAfter, CryptoDirection.Decrypt)
        {
            CryptoName = "DES";
            SourceFilePath = filePath;
            DestFilePath = decryptFilePath;

            if (multithread)
                StartMultithread(DES_.GetNice(key56, CryptoDirection.Decrypt));
            else
                Start(DES_.Get(key56, CryptoDirection.Decrypt));
        }

    }
}
