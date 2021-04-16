using CryptographyLabs.Crypto;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows;

namespace CryptographyLabs.GUI
{
    abstract class RijndaelVM : BaseViewModel
    {
        #region Bindings

        private Rijndael_.Mode _mode = Rijndael_.Mode.ECB;
        public Rijndael_.Mode Mode => _mode;
        public int ModeIndex
        {
            get => (int)_mode;
            set
            {
                _mode = (Rijndael_.Mode)value;
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

        private Rijndael_.Size _blockSize;
        public Rijndael_.Size BlockSize => _blockSize;
        public int BlockSizeIndex
        {
            get => (int)_blockSize;
            set
            {
                _blockSize = (Rijndael_.Size)value;
                NotifyPropChanged(nameof(BlockSizeIndex));
            }
        }

        private Rijndael_.Size _keySize;
        public Rijndael_.Size KeySize => _keySize;
        public int KeySizeIndex
        {
            get => (int)_keySize;
            set
            {
                _keySize = (Rijndael_.Size)value;
                NotifyPropChanged(nameof(KeySizeIndex));
            }
        }

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

        private string _IV = "";
        public string IV
        {
            get => _IV;
            set
            {
                _IV = value;
                NotifyPropChanged(nameof(IV));
            }
        }

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

        private RelayCommand _loadKeyCmd;
        public RelayCommand LoadKeyCmd
            => _loadKeyCmd ?? (_loadKeyCmd = new RelayCommand(_ => LoadKey()));

        private RelayCommand _saveKeyCmd;
        public RelayCommand SaveKeyCmd
            => _saveKeyCmd ?? (_saveKeyCmd = new RelayCommand(_ => SaveKey()));

        private RelayCommand _goCmd;
        public RelayCommand GoCmd
            => _goCmd ?? (_goCmd = new RelayCommand(_ => Go()));

        #endregion

        private string _jKeyForKey = "key";
        private string _jKeyForIV = "iv";

        protected abstract void ChangeFilePath();

        protected abstract void Go();

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
                MessageBox.Show($"Error: {e.Message}");
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
                MessageBox.Show($"Error: {e.Message}");
            }
        }

    }
}
