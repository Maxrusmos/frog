using System;
using System.Collections.Generic;
using System.Text;

namespace CryptographyLabs.Crypto
{
    public partial class FROGProvider
    {
        public enum Mode
        {
            ECB = 0,
            CBC = 1,
            CFB = 2,
            OFB = 3
        }
    }
}
