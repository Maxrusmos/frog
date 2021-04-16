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
    class VernamDecryptVM : BaseTransformVM
    {
        public VernamDecryptVM(string filePath, string destFilePath, string keyFilePath, bool isDeleteAfter)
            : base(isDeleteAfter, CryptoDirection.Decrypt)
        {
            SourceFilePath = filePath;
            DestFilePath = destFilePath;
            CryptoName = "Vernam";

            try
            {
                FileStream inKeyStream = new FileStream(keyFilePath, FileMode.Open, FileAccess.Read);
                Start(Vernam.Get(inKeyStream));
            }
            catch (Exception e)
            {
                StatusString = "Error open key stream: " + e.Message;
            }
        }
    }
}
