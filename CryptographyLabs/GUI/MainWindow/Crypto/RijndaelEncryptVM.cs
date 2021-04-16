using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptographyLabs.Crypto;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CryptographyLabs.GUI
{
    class RijndaelEncryptVM : RijndaelVM
    {
        private MainWindowVM _owner;

        public RijndaelEncryptVM(MainWindowVM owner)
        {
            _owner = owner;
        }

        protected override void ChangeFilePath()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.Filters.Add(new CommonFileDialogFilter("Any file", "*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    FilePath = dialog.FileName;
            }
        }

        protected override void Go()
        {
            if (!StringEx.TryParse(Key, out byte[] keyBytes))
            {
                MessageBox.Show("Wrong key format.");
                return;
            }
            if (keyBytes.Length != Rijndael_.GetBytesCount(KeySize))
            {
                MessageBox.Show("Wrong bytes count in key.");
                return;
            }

            string encryptPath = FilePath + ".rjn399";
            BaseTransformVM vm;
            if (Mode == Rijndael_.Mode.ECB)
            {
                vm = new RijndaelEncryptTransformVM(FilePath, encryptPath, keyBytes, BlockSize, IsDeleteAfter,
                    Multithread);
            }
            else
            {
                if (!StringEx.TryParse(IV, out byte[] iv))
                {
                    MessageBox.Show("Wrong IV format.");
                    return;
                }
                if (iv.Length != Rijndael_.GetBytesCount(BlockSize))
                {
                    MessageBox.Show($"Wrong IV bytes count. Must be {Rijndael_.GetBytesCount(BlockSize)}.");
                    return;
                }

                vm = new RijndaelEncryptTransformVM(FilePath, encryptPath, keyBytes, 
                    BlockSize, iv, Mode, IsDeleteAfter);
            }

            _owner.ProgressViewModels.Add(vm);
        }
    }
}
