using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.GUI
{
    class Task1_14VM : BaseViewModel
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

        private string _m = "";
        public string M
        {
            get => _m;
            set
            {
                _m = value;
                NotifyPropChanged(nameof(M));
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
            if (StringEx.TryParse(A, out uint a) && int.TryParse(M, out int m))
            {
                if (m >= 0 && m <= 32)
                    Result = "0b" + Convert.ToString(Bitops.NullifyMLowBits(a, m), 2);
                else
                    Result = "-";
            }
            else
            {
                Result = "-";
            }
        }
    }
}
