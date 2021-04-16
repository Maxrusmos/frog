using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.GUI
{
    class Task1_3VM : BaseViewModel
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

        private string _permutation = "";
        public string Permutation
        {
            get => _permutation;
            set
            {
                _permutation = value;
                NotifyPropChanged(nameof(Permutation));
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
            string[] items = Permutation.Split(new string[] { " ", ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (StringEx.TryParse(A, out uint a) && items.Length == 4)
            {
                byte[] indices = new byte[4];
                for (int i = 0; i < 4; ++i)
                {
                    if (!byte.TryParse(items[i], out indices[i]))
                    {
                        Result = "-";
                        return;
                    }
                }
                try
                {
                    Result = "0x" + Convert.ToString(Bitops.BytePermutation(a, indices), 16).ToUpper();
                }
                catch (ArgumentException)
                {
                    Result = "-";
                }
            }
            else
                Result = "-";
        }

    }
}
