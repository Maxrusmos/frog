using CryptographyLabs.Crypto;
using CryptographyLabs.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.GUI
{
    class Task3_SpecVM : BaseViewModel
    {

        #region Bindings

        private RelayCommand _generateSBoxCmd;
        public RelayCommand GenerateSBoxCmd
            => _generateSBoxCmd ?? (_generateSBoxCmd = new RelayCommand(_ => GenerateSBox()));

        private RelayCommand _generateInvSBoxCmd;
        public RelayCommand GenerateInvSBoxCmd
            => _generateInvSBoxCmd ?? (_generateInvSBoxCmd = new RelayCommand(_ => GenerateInvSBox()));

        private RelayCommand _generateInvMatrixCmd;
        public RelayCommand GenerateInvMatrixCmd
            => _generateInvMatrixCmd ?? (_generateInvMatrixCmd = new RelayCommand(_ => GenerateInvMixColumnMtx()));

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

        private void GenerateSBox()
        {
            byte[] sBox = Rijndael_.GenerateSBox();
            string sBoxStr = ArrayEx.ToGridString(sBox, 16, b => $"0x{Convert.ToString(b, 16).PadLeft(2, '0')}");
            Result = "SBox:\n" + sBoxStr;
        }

        private void GenerateInvSBox()
        {
            byte[] invSBox = Rijndael_.GenerateInvSBox();
            string gridStr = ArrayEx.ToGridString(invSBox, 16, 
                b => $"0x{Convert.ToString(b, 16).PadLeft(2, '0')}");
            Result = "InvSBox:\n" + gridStr;
        }

        private void GenerateInvMixColumnMtx()
        {
            byte[][] invMtx = Rijndael_.GenerateInvMixColumnsMtx();
            string gridStr = ArrayEx.ToGridString(invMtx, b => $"0x{Convert.ToString(b, 16).PadLeft(2, '0')}");
            Result = "Inverse mix columns matrix:\n" + gridStr;
        }
    }
}
