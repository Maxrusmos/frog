using CryptographyLabs.Crypto;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CryptographyLabs.GUI
{
    public class RC4VM : BaseViewModel
    {
        private MainWindowVM _owner;

        #region Bindings

        private string _filename = "";
        public string Filename
        {
            get => _filename;
            set
            {
                _filename = value;
                NotifyPropChanged(nameof(Filename));
            }
        }

        private string _key = "";
        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                NotifyPropChanged(nameof(Key));
            }
        }

        private bool _isDeleteFileAfter = false;
        public bool IsDeleteFileAfter
        {
            get => _isDeleteFileAfter;
            set
            {
                _isDeleteFileAfter = value;
                NotifyPropChanged(nameof(IsDeleteFileAfter));
            }
        }

        private RelayCommand _changeFilenameCommand;
        public RelayCommand ChangeFilenameCommand =>
            _changeFilenameCommand ?? (_changeFilenameCommand = new RelayCommand(_ => ChangeFilename()));

        private RelayCommand _goCommand;
        public RelayCommand GoCommand =>
            _goCommand ?? (_goCommand = new RelayCommand(_ => Go()));

        #endregion

        public RC4VM(MainWindowVM owner)
        {
            _owner = owner;
        }

        private void ChangeFilename()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.Filters.Add(new CommonFileDialogFilter("Any file", "*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    Filename = dialog.FileName;
            }
        }

        private void Go()
        {
            byte[] keyBytes;
            if (!StringEx.TryParse(Key, out keyBytes))
            {
                MessageBox.Show("Wrong key format.", "Input error");
                return;
            }
            if (keyBytes.Length < 1 || keyBytes.Length > 256)
            {
                MessageBox.Show("Size of key must be 1 to 256 bytes.", "Input error");
                return;
            }

            string filePath = Filename;
            bool isDeleteAfter = IsDeleteFileAfter;

            string destFilename;
            if (filePath.EndsWith(".rc4399"))
                destFilename = filePath.Substring(0, filePath.Length - 7);
            else
                destFilename = filePath + ".rc4399";

            var vm = new RC4CryptVM(filePath, destFilename, keyBytes, isDeleteAfter);
            _owner.ProgressViewModels.Add(vm);
        }

    }
}
