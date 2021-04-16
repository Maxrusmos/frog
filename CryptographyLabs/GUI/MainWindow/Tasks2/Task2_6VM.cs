using CryptographyLabs.Crypto;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Windows;

namespace CryptographyLabs.GUI
{
    public class Task2_6VM : BaseViewModel
    {
        #region Bindings

        private string _aStr;
        public string AStr
        {
            get => _aStr;
            set
            {
                _aStr = value;
                NotifyPropChanged(nameof(AStr));
            }
        }

        private string _bStr;
        public string BStr
        {
            get => _bStr;
            set
            {
                _bStr = value;
                NotifyPropChanged(nameof(BStr));
            }
        }

        private RelayCommand _goCmd;
        public RelayCommand GoCmd
            => _goCmd ?? (_goCmd = new RelayCommand(_ => Go()));

        private string _gcd;
        public string GCD
        {
            get => _gcd;
            set
            {
                _gcd = value;
                NotifyPropChanged(nameof(GCD));
            }
        }

        private string _x;
        public string X
        {
            get => _x;
            set
            {
                _x = value;
                NotifyPropChanged(nameof(X));
            }
        }

        private string _y;
        public string Y
        {
            get => _y;
            set
            {
                _y = value;
                NotifyPropChanged(nameof(Y));
            }
        }

        #endregion

        private void Go()
        {
            if (!BigInteger.TryParse(AStr, out BigInteger a))
            {
                MessageBox.Show("Wrong format of a.");
                return;
            }
            if (!BigInteger.TryParse(BStr, out BigInteger b))
            {
                MessageBox.Show("Wrong format of b.");
                return;
            }
            if (a <= 0 || b <= 0)
            {
                MessageBox.Show("Values must be > 0.");
                return;
            }

            BigInteger gcd = RSAAlg.GCD(a, b, out BigInteger x, out BigInteger y);
            GCD = gcd.ToString();
            X = x.ToString();
            Y = y.ToString();
        }
    }
}
