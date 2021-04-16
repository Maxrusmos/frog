using CryptographyLabs.Crypto;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CryptographyLabs.GUI
{
    public class FrogDecryptVM : BaseViewModel
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

        public FrogDecryptVM(FrogVM owner)
        {
            _owner = owner;
        }

        private void ChangeFilePath()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.Filters.Add(new CommonFileDialogFilter("Encrypted file", ".frg399"));
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

            string decryptPath;
            if (FilePath.EndsWith(".frg399"))
                decryptPath = FilePath.Substring(0, FilePath.Length - 7);
            else
            {
                MessageBox.Show("Wrong extenstion of encrypted file. Must be \".frg399\".");
                return;
            }

            BaseTransformVM vm;
            if (_owner.Mode == FROGProvider.Mode.ECB)
            {
                vm = new FrogDecryptTransformVM(FilePath, decryptPath, keyBytes,
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
                    MessageBox.Show($"Wrong IV bytes count. Must be {FROGProvider.BlockSize}.");
                    return;
                }

                vm = new FrogDecryptTransformVM(FilePath, decryptPath, keyBytes,
                    _owner.Mode, iv, _owner.IsDeleteAfter);
            }

            _owner.Owner.ProgressViewModels.Add(vm);
        }
    }
}
