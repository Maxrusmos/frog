using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptographyLabs.Crypto;

namespace CryptographyLabs.GUI
{
    class RijndaelEncryptTransformVM : BaseTransformVM
    {
        public RijndaelEncryptTransformVM(string filePath, string encryptFilePath, byte[] key, 
            Rijndael_.Size blockSize, bool isDeleteAfter, bool multithread = false) 
            : base(isDeleteAfter, CryptoDirection.Encrypt)
        {
            CryptoName = "Rijndael";
            SourceFilePath = filePath;
            DestFilePath = encryptFilePath;

            if (multithread)
                StartMultithread(Rijndael_.GetNice(key, blockSize, CryptoDirection.Encrypt));
            else
                Start(Rijndael_.Get(key, blockSize, CryptoDirection.Encrypt));
        }

        public RijndaelEncryptTransformVM(string filePath, string encryptFilePath, byte[] key,
            Rijndael_.Size blockSize, byte[] IV, Rijndael_.Mode mode, bool isDeleteAfter)
            : base(isDeleteAfter, CryptoDirection.Encrypt)
        {
            CryptoName = "Rijndael";
            SourceFilePath = filePath;
            DestFilePath = encryptFilePath;

            Start(Rijndael_.Get(key, blockSize, IV, mode, CryptoDirection.Encrypt));
        }


    }
}
