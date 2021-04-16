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
    public class VernamVM : BaseViewModel
    {
        private MainWindowVM _owner;

        #region Bindings

        private bool _isEncrypt = true;
        public bool IsEncrypt
        {
            get => _isEncrypt;
            set
            {
                _isEncrypt = value;
                NotifyPropChanged(nameof(IsEncrypt));
            }
        }

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

        private string _keyFilename = "";
        public string KeyFilename
        {
            get => _keyFilename;
            set
            {
                _keyFilename = value;
                NotifyPropChanged(nameof(KeyFilename));
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

        private RelayCommand _changeKeyFilenameCommand;
        public RelayCommand ChangeKeyFilenameCommand =>
            _changeKeyFilenameCommand ?? (_changeKeyFilenameCommand = new RelayCommand(_ => ChangeKeyFilename()));

        private RelayCommand _goCommand;
        public RelayCommand GoCommand =>
            _goCommand ?? (_goCommand = new RelayCommand(_ => Go()));

        #endregion

        public VernamVM(MainWindowVM owner)
        {
            _owner = owner;
        }

        private void ChangeFilename()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                if (!IsEncrypt)
                    dialog.Filters.Add(new CommonFileDialogFilter("Encrypted file", ".v399"));
                dialog.Filters.Add(new CommonFileDialogFilter("Any file", "*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    Filename = dialog.FileName;
            }
        }

        private void ChangeKeyFilename()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.Filters.Add(new CommonFileDialogFilter("Key file", ".vkey399"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    KeyFilename = dialog.FileName;
            }
        }

        private void Go()
        {
            if (IsEncrypt)
            {
                string encryptPath = Filename + ".v399";
                string keyFilePath = Filename + ".vkey399";

                var vm = new VernamEncryptVM(Filename, encryptPath, keyFilePath, IsDeleteFileAfter);
                _owner.ProgressViewModels.Add(vm);
            }
            else
            {
                string decryptPath;
                if (Filename.EndsWith(".v399"))
                    decryptPath = Filename.Substring(0, Filename.Length - 5);
                else
                {
                    MessageBox.Show("Wrong file extension.");
                    return;
                }

                var vm = new VernamDecryptVM(Filename, decryptPath, KeyFilename, IsDeleteFileAfter);
                _owner.ProgressViewModels.Add(vm);
            }
        }
    }
}
