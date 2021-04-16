using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using CryptographyLabs.Crypto;

namespace CryptographyLabs.GUI
{
    public class DesDecryptVM : BaseViewModel
    {
        private MainWindowVM _owner;

        private DesVM _desVM;
        public DesVM DesVM => _desVM;

        public DesDecryptVM(DesVM desVM, MainWindowVM owner)
        {
            _desVM = desVM;
            _owner = owner;
        }

        #region Bindings

        private string _filenameToDecrypt = "";
        public string FilenameToDecrypt
        {
            get => _filenameToDecrypt;
            set
            {
                _filenameToDecrypt = value;
                NotifyPropChanged(nameof(FilenameToDecrypt));
            }
        }

        private RelayCommand _changeFilenameCmd;
        public RelayCommand ChangeFilenameCmd =>
            _changeFilenameCmd ?? (_changeFilenameCmd = new RelayCommand(_ => ChangeFilename()));

        private RelayCommand _goDecryptCmd;
        public RelayCommand GoDecryptCmd
            => _goDecryptCmd ?? (_goDecryptCmd = new RelayCommand(_ => GoDecrypt()));

        #endregion

        private void ChangeFilename()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.Filters.Add(new CommonFileDialogFilter("Encrypted file", ".des399"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    FilenameToDecrypt = dialog.FileName;
            }
        }

        private void GoDecrypt()
        {
            if (!StringEx.TryParse(DesVM.Key, out ulong key56))
            {
                MessageBox.Show("Wrong key format.", "Error");
                return;
            }

            string filePath = FilenameToDecrypt;

            string decryptPath;
            if (filePath.EndsWith(".des399"))
                decryptPath = filePath.Substring(0, filePath.Length - 7);
            else
            {
                MessageBox.Show("Wrong extension of file.");
                return;
            }

            BaseTransformVM vm;
            if (DesVM.Mode == DES_.Mode.ECB)
            {
                vm = new DESDecryptTransformVM(filePath, decryptPath, key56, DesVM.IsDeleteFileAfter, 
                    DesVM.Multithreading);
            }
            else
            {
                if (!StringEx.TryParse(DesVM.IV, out byte[] IV))
                {
                    MessageBox.Show("Wrong IV format.");
                    return;
                }
                if (IV.Length != DES_.BlockSize)
                {
                    MessageBox.Show($"Wrong IV bytes count. Must be {DES_.BlockSize}.");
                    return;
                }

                vm = new DESDecryptTransformVM(filePath, decryptPath, key56, IV, DesVM.Mode, DesVM.IsDeleteFileAfter);
            }

            _owner.ProgressViewModels.Add(vm);
        }
    }
}
