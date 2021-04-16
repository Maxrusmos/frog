using CryptographyLabs.Crypto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CryptographyLabs.GUI
{
    public class Task2_2VM : BaseViewModel
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
            if (!ulong.TryParse(MStr, out ulong m))
            {
                MessageBox.Show("Wrong format of m.");
                return;
            }
            if (m < 2)
            {
                MessageBox.Show("m must be >= 2.");
                return;
            }

            List<ulong> system = RSAAlg.ReducedDeductionSystem(m);

            StringBuilder builder = new StringBuilder();
            bool isFirst = true;
            foreach (ulong value in system)
            {
                if (isFirst)
                    isFirst = false;
                else
                    builder.Append("; ");
                builder.Append(value);
            }
            Result = builder.ToString();
        }

    }
}
