using CryptographyLabs.Crypto;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Windows;

namespace CryptographyLabs.GUI
{
    public class Task2_5VM : BaseViewModel
    {
        #region Bindings

        private string _valueStr = "";
        public string ValueStr
        {
            get => _valueStr;
            set
            {
                _valueStr = value;
                NotifyPropChanged(nameof(ValueStr));
            }
        }

        private string _powerStr = "";
        public string PowerStr
        {
            get => _powerStr;
            set
            {
                _powerStr = value;
                NotifyPropChanged(nameof(PowerStr));
            }
        }

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
            if (!BigInteger.TryParse(ValueStr, out BigInteger value))
            {
                MessageBox.Show("Wrong format of value.");
                return;
            }
            if (!BigInteger.TryParse(PowerStr, out BigInteger power))
            {
                MessageBox.Show("Wrong format of power.");
                return;
            }
            if (!BigInteger.TryParse(MStr, out BigInteger m))
            {
                MessageBox.Show("Wrong format of m.");
                return;
            }
            if (value == 0 && power == 0)
            {
                MessageBox.Show("Value and power = 0...");
                return;
            }
            if (power < 0)
            {
                MessageBox.Show("Power must be >= 0.");
                return;
            }
            if (m < 1)
            {
                MessageBox.Show("m must be > 0.");
                return;
            }

            BigInteger result = RSAAlg.BinPowMod(value, power, m);
            Result = result.ToString();
        }
    }
}
