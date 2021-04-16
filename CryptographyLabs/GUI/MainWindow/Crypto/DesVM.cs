using CryptographyLabs.Crypto;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class DesVM : BaseViewModel
    {
        #region Bindings

        private DES_.Mode _mode = DES_.Mode.ECB;
        public DES_.Mode Mode => _mode;
        public int ModeIndex
        {
            get => (int)_mode;
            set
            {
                _mode = (DES_.Mode)value;
                NotifyPropChanged(nameof(ModeIndex), nameof(Mode));
            }
        }

        private bool _multithreading = false;
        public bool Multithreading
        {
            get => _multithreading;
            set
            {
                _multithreading = value;
                NotifyPropChanged(nameof(Multithreading));
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

        private RelayCommand _loadKeyCmd;
        public RelayCommand LoadKeyCmd
            => _loadKeyCmd ?? (_loadKeyCmd = new RelayCommand(_ => LoadKey()));

        private RelayCommand _saveKeyCmd;
        public RelayCommand SaveKeyCmd
            => _saveKeyCmd ?? (_saveKeyCmd = new RelayCommand(_ => SaveKey()));

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

        #endregion

        private string _jKeyForKey = "key";
        private string _jKeyForIV = "iv";

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
            //dialog.Filters.Add(new CommonFileDialogFilter("Json", "*.json"));
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
