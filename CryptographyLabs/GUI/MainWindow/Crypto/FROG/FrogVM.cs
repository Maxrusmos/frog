using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using CryptographyLabs.Crypto;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptographyLabs.GUI
{
    public class FrogVM : BaseViewModel
    {

        #region Bindings

        private bool _isEncrypt = true;
        public bool IsEncrypt
        {
            get => _isEncrypt;
            set
            {
                _isEncrypt = value;
                if (value)
                    CurrentTransform = _encryptTransform;
                else
                    CurrentTransform = _decryptTransform;

                NotifyPropChanged(nameof(IsEncrypt));
            }
        }

        private FROGProvider.Mode _mode = FROGProvider.Mode.ECB;
        public FROGProvider.Mode Mode => _mode;
        public int ModeIndex
        {
            get => (int)_mode;
            set
            {
                _mode = (FROGProvider.Mode)value;
                NotifyPropChanged(nameof(ModeIndex), nameof(Mode));
            }
        }

        private bool _multithread = false;
        public bool Multithread
        {
            get => _multithread;
            set
            {
                _multithread = value;
                NotifyPropChanged(nameof(Multithread));
            }
        }

        BaseViewModel _encryptTransform;
        BaseViewModel _decryptTransform;

        private BaseViewModel _currentTransform;
        public BaseViewModel CurrentTransform
        {
            get => _currentTransform ?? (_currentTransform = _encryptTransform);
            set
            {
                _currentTransform = value;
                NotifyPropChanged(nameof(CurrentTransform));
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

        private string _iv = "";
        public string IV
        {
            get => _iv;
            set
            {
                _iv = value;
                NotifyPropChanged(nameof(IV));
            }
        }

        private RelayCommand _loadKeyCmd;
        public RelayCommand LoadKeyCmd
            => _loadKeyCmd ?? (_loadKeyCmd = new RelayCommand(_ => LoadKey()));

        private RelayCommand _saveKeyCmd;
        public RelayCommand SaveKeyCmd
            => _saveKeyCmd ?? (_saveKeyCmd = new RelayCommand(_ => SaveKey()));

        private bool _isDeleteAfter = false;
        public bool IsDeleteAfter
        {
            get => _isDeleteAfter;
            set
            {
                _isDeleteAfter = value;
                NotifyPropChanged(nameof(IsDeleteAfter));
            }
        }

        #endregion

        private MainWindowVM _owner;
        public MainWindowVM Owner => _owner;

        private string _jKeyForKey = "key";
        private string _jKeyForIV = "iv";

        public FrogVM(MainWindowVM owner)
        {
            _owner = owner;

            _encryptTransform = new FrogEncryptVM(this);
            _decryptTransform = new FrogDecryptVM(this);
        }

        private void LoadKey()
        {
            using var dialog = new CommonOpenFileDialog();
            dialog.EnsureFileExists = true;
            dialog.Filters.Add(new CommonFileDialogFilter("Json", "*.json"));
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;

            try
            {
                string text = File.ReadAllText(dialog.FileName);
                JObject obj = (JObject)JsonConvert.DeserializeObject(text);
                string key = obj.Value<string>(_jKeyForKey);
                string iv = obj.Value<string>(_jKeyForIV);
                Key = key;
                IV = iv;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error load key: {e.Message}");
            }
        }

        private void SaveKey()
        {
            using var dialog = new CommonSaveFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("Json file", ".json"));
            dialog.DefaultExtension = "json";
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;

            JObject obj = new JObject(new object[]
            {
                new JProperty(_jKeyForKey, Key),
                new JProperty(_jKeyForIV, IV)
            });

            try
            {
                File.WriteAllText(dialog.FileName, obj.ToString(Formatting.Indented));
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error save key: {e.Message}");
            }
        }
    }
}
