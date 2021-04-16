using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.GUI
{
    class Task1_6VM : BaseViewModel
    {
        private string _a = "";
        public string A
        {
            get => _a;
            set
            {
                _a = value;
                NotifyPropChanged(nameof(A));
                Apply();
            }
        }

        private string _p = "";
        public string P
        {
            get => _p;
            set
            {
                _p = value;
                NotifyPropChanged(nameof(P));
                Apply();
            }
        }

        private string _result = "";
        public string Result
        {
            get => _result;
            set
            {
                _result = value;
                NotifyPropChanged(nameof(Result));
            }
        }

        private void Apply()
        {
            if (StringEx.TryParse(A, out uint a) && byte.TryParse(P, out byte p))
            {
                Result = Bitops.XorBits(a, p).ToString();
            }
            else
                Result = "-";
        }
    }
}
