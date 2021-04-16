using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.GUI
{
    class Task3_1VM : BaseViewModel
    {
        #region Bindings

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
                Result = StringEx.AsPolynom(value);
            else
                Result = "-";
        }
    }
}
