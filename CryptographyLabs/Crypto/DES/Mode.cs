using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.Crypto
{
    public static partial class DES_
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
