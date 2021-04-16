using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptographyLabs.Crypto;

namespace CryptographyLabs.GUI
{
    class RijndaelDecryptTransformVM : BaseTransformVM
    {
        public RijndaelDecryptTransformVM(string filePath, string decryptFilePath, byte[] key, 
            Rijndael_.Size blockSize, bool isDeleteAfter, bool multithread = false) 
            : base(isDeleteAfter, CryptoDirection.Decrypt)
        {
            CryptoName = "Rijndael";
            SourceFilePath = filePath;
            DestFilePath = decryptFilePath;

            if (multithread)
                StartMultithread(Rijndael_.GetNice(key, blockSize, CryptoDirection.Decrypt));
            else
                Start(Rijndael_.Get(key, blockSize, CryptoDirection.Decrypt));
        }

        public RijndaelDecryptTransformVM(string filePath, string decryptFilePath, byte[] key,
            Rijndael_.Size blockSize, byte[] IV, Rijndael_.Mode mode, bool isDeleteAfter)
            : base(isDeleteAfter, CryptoDirection.Decrypt)
        {
            CryptoName = "Rijndael";
            SourceFilePath = filePath;
            DestFilePath = decryptFilePath;

            Start(Rijndael_.Get(key, blockSize, IV, mode, CryptoDirection.Decrypt));
        }

    }
}
