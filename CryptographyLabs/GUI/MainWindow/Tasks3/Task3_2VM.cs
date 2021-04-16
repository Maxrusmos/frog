using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.GUI
{
    class Task3_2VM : BaseViewModel
    {
        #region Bindings

        private string _firstGfElement = "";
        public string FirstGFElement
        {
            get => _firstGfElement;
            set
            {
                _firstGfElement = value;
                NotifyPropChanged(nameof(FirstGFElement));
                Apply();
            }
        }

        private string _secondGfElement = "";
        public string SecondGFElement
        {
            get => _secondGfElement;
            set
            {
                _secondGfElement = value;
                NotifyPropChanged(nameof(SecondGFElement));
                Apply();
            }
        }

        private string _polyResult;
        public string PolyResult
        {
            get => _polyResult;
            set
            {
                _polyResult = value;
                NotifyPropChanged(nameof(PolyResult));
            }
        }

        private string _gfResult;
        public string GFResult
        {
            get => _gfResult;
            set
            {
                _gfResult = value;
                NotifyPropChanged(nameof(GFResult));
            }
        }

        #endregion

        private void Apply()
        {
            if (!StringEx.TryParse(FirstGFElement, out byte a) || !StringEx.TryParse(SecondGFElement, out byte b))
            {
                PolyResult = "-";
                GFResult = "-";
                return;
            }

            ushort polyRes = BinPoly.Multiply(a, b);
            byte gfRes = GF.Multiply(a, b, false);

            PolyResult = BinPoly.ToAllStrRepr(polyRes);
            GFResult = BinPoly.ToAllStrRepr(gfRes);
        }
    }
}
