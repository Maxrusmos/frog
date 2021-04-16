using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.GUI
{
    class Task1_8VM : BaseViewModel
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
            if (StringEx.TryParse(A, out ulong a) && items.Length > 0 && items.Length <= 64)
            {
                byte[] permutation = new byte[items.Length];
                for (int i = 0; i < permutation.Length; ++i)
                {
                    if (!byte.TryParse(items[i], out permutation[i]))
                    {
                        Result = "-";
                        return;
                    }
                }

                try
                {
                    Result = "0b" + Convert.ToString((long)Bitops.BitPermutation(a, permutation), 2);
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
