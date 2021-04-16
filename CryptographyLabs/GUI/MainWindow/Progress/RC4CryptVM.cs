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
    class RC4CryptVM : BaseTransformVM
    {
        public RC4CryptVM(string sourceFilePath, string destFilePath, byte[] keyBytes, bool isDeleteAfter)
            : base(isDeleteAfter, null)
        {

            SourceFilePath = sourceFilePath;
            DestFilePath = destFilePath;
            CryptoName = "RC4";

            Start(RC4.Get(keyBytes));
        }
    }
}
