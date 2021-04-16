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
    class VernamEncryptVM : BaseTransformVM
    {

        string _keyFilePath;

        public VernamEncryptVM(string filePath, string destFilePath, string keyFilePath, bool isDeleteAfter)
            : base(isDeleteAfter, CryptoDirection.Encrypt)
        {
            SourceFilePath = filePath;
            DestFilePath = destFilePath;
            CryptoName = "Vernam";

            _keyFilePath = keyFilePath;

            Start();
        }

        private async void Start()
        {
            StatusString = "Generating key file...";
            try
            {
                long bytesCount = new FileInfo(SourceFilePath).Length;
                await Vernam.GenerateKeyFileAsync(_keyFilePath, bytesCount, 80_000);
                FileStream inKeyStream = new FileStream(_keyFilePath, FileMode.Open, FileAccess.Read);
                Start(Vernam.Get(inKeyStream));
            }
            catch (Exception e)
            {
                StatusString = "Error: " + e.Message;
            }
        }

    }
}
