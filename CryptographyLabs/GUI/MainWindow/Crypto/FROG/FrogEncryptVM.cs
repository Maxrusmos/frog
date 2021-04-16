using CryptographyLabs.Crypto;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CryptographyLabs.GUI
{
    public class FrogEncryptVM : BaseViewModel
    {
        #region Bindings

        private string _filePath = "";
        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                NotifyPropChanged(nameof(FilePath));
            }
        }

        private RelayCommand _changeFilePathCmd;
        public RelayCommand ChangeFilePathCmd
            => _changeFilePathCmd ?? (_changeFilePathCmd = new RelayCommand(_ => ChangeFilePath()));

        private RelayCommand _goCmd;
        public RelayCommand GoCmd
            => _goCmd ?? (_goCmd = new RelayCommand(_ => Go()));

        #endregion

        private FrogVM _owner;

        public FrogEncryptVM(FrogVM owner)
        {
            _owner = owner;
        }

        private void ChangeFilePath()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.Filters.Add(new CommonFileDialogFilter("Any file", "*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    FilePath = dialog.FileName;
            }
        }

        private void Go()
        {
            if (!StringEx.TryParse(_owner.Key, out byte[] keyBytes))
            {
                MessageBox.Show("Wrong key format.");
                return;
            }
            if (keyBytes.Length < FROGProvider.MinKeyLength || keyBytes.Length > FROGProvider.MaxKeyLength)
            {
                MessageBox.Show($"Wrong length of key. " +
                    $"Min length: {FROGProvider.MinKeyLength}. Max length: {FROGProvider.MaxKeyLength}.");
                return;
            }

            string encryptFilePath = FilePath + ".frg399";
            BaseTransformVM vm;
            if (_owner.Mode == FROGProvider.Mode.ECB)
            {
                vm = new FrogEncryptTransformVM(FilePath, encryptFilePath, keyBytes,
                    _owner.IsDeleteAfter, _owner.Multithread);
            }
            else
            {
                if (!StringEx.TryParse(_owner.IV, out byte[] iv))
                {
                    MessageBox.Show("Wrong IV format.");
                    return;
                }
                if (iv.Length != FROGProvider.BlockSize)
                {
                    MessageBox.Show($"Wrong length of IV. Must be {FROGProvider.BlockSize}.");
                    return;
                }

                vm = new FrogEncryptTransformVM(FilePath, encryptFilePath, keyBytes,
                    _owner.Mode, iv, _owner.IsDeleteAfter);
            }

            _owner.Owner.ProgressViewModels.Add(vm);
        }
    }
}
