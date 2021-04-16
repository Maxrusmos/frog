using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.GUI
{
    class Task1_21VM : BaseViewModel
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

        private string _len = "";
        public string Len
        {
            get => _len;
            set
            {
                _len = value;
                NotifyPropChanged(nameof(Len));
                Apply();
            }
        }

        private string _i = "";
        public string I
        {
            get => _i;
            set
            {
                _i = value;
                NotifyPropChanged(nameof(I));
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
            if (StringEx.TryParse(A, out uint a) && int.TryParse(Len, out int len) && int.TryParse(I, out int i))
            {
                try
                {
                    Result = "0b" + Convert.ToString(Bitops.ConcatExtremeBits(a, len, i), 2);
                }
                catch (ArgumentException)
                {
                    Result = "-";
                }
            }
            else
            {
                Result = "-";
            }
        }
    }
}
