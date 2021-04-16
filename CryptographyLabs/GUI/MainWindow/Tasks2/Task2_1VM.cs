using CryptographyLabs.Crypto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CryptographyLabs.GUI
{
    public class Task2_1VM : BaseViewModel
    {
        #region Bindings

        private string _mStr = "";
        public string MStr
        {
            get => _mStr;
            set
            {
                _mStr = value;
                NotifyPropChanged(nameof(MStr));
            }
        }

        private RelayCommand _goCmd;
        public RelayCommand GoCmd
            => _goCmd ?? (_goCmd = new RelayCommand(_ => Go()));

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

        #endregion

        private void Go()
        {
            if (!int.TryParse(MStr, out int m))
            {
                MessageBox.Show("Wrong format of m.");
                return;
            }
            if (m <= 2)
            {
                MessageBox.Show("m must be > 2.");
                return;
            }

            List<int> primes = RSAAlg.CalcPrimes(m);
            StringBuilder builder = new StringBuilder();
            bool isFirst = true;
            foreach (int prime in primes)
            {
                if (isFirst)
                    isFirst = false;
                else
                    builder.Append(", ");
                builder.Append(prime);
            }
            Result = builder.ToString();
        }
    }
}
