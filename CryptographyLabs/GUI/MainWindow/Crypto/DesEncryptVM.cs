using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using CryptographyLabs.Crypto;

namespace CryptographyLabs.GUI
{
    public class DesEncryptVM : BaseViewModel
    {
        private MainWindowVM _owner;

        private DesVM _desVM;
        public DesVM DesVM => _desVM;

        public DesEncryptVM(DesVM desVM, MainWindowVM owner)
        {
            _desVM = desVM;
            _owner = owner;
        }

        #region Bindings

        private string _filenameToEncrypt = "";
        public string FilenameToEncrypt
        {
            get => _filenameToEncrypt;
            set
            {
                _filenameToEncrypt = value;
                NotifyPropChanged(nameof(FilenameToEncrypt));
            }
        }

        private RelayCommand _changeFilenameCmd;
        public RelayCommand ChangeFilenameCmd =>
            _changeFilenameCmd ?? (_changeFilenameCmd = new RelayCommand(_ => ChangeFilename()));

        private RelayCommand _goEncryptCmd;
        public RelayCommand GoEncryptCmd
            => _goEncryptCmd ?? (_goEncryptCmd = new RelayCommand(_ => GoEncrypt()));

        #endregion

        private void ChangeFilename()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.Filters.Add(new CommonFileDialogFilter("Any file", "*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    FilenameToEncrypt = dialog.FileName;
            }
        }

        private void GoEncrypt()
        {
            ulong key56;
            if (!StringEx.TryParse(DesVM.Key, out key56))
            {
                MessageBox.Show("Wrong key format.", "Error");
                return;
            }

            string filePath = FilenameToEncrypt;
            string encryptPath = filePath + ".des399";

            BaseTransformVM vm;
            if (DesVM.Mode == DES_.Mode.ECB)
            {
                vm = new DESEncryptTransformVM(filePath, encryptPath, key56, DesVM.IsDeleteFileAfter, 
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

                vm = new DESEncryptTransformVM(filePath, encryptPath, key56, IV, DesVM.Mode, DesVM.IsDeleteFileAfter);
            }

            _owner.ProgressViewModels.Add(vm);
        }
    }
}
