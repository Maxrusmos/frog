using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.GUI
{
    class Task3_3VM : BaseViewModel
    {
        #region Bindigns

        private string _gfElement = "";
        public string GFElement
        {
            get => _gfElement;
            set
            {
                _gfElement = value;
                NotifyPropChanged(nameof(GFElement));
                Apply();
            }
        }

        private string _result;
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

        private void Apply()
        {
            if (StringEx.TryParse(GFElement, out byte value))
            {
                byte inversed = GF.Inverse(value, false);
                Result = BinPoly.ToAllStrRepr(inversed);
            }
            else
                Result = "-";
        }
    }
}
