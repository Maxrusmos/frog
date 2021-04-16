using CryptographyLabs.Crypto;
using CryptographyLabs.Crypto.BlockCouplingModes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptographyLabs.GUI
{
    public abstract class BaseTransformVM : BaseViewModel
    {
        protected CancellationTokenSource _cts = new CancellationTokenSource();

        #region Bindings

        private long _startTime = DateTime.Now.Ticks;
        public long StartTime => _startTime;

        private string _sourceFilePath = "";
        public string SourceFilePath
        {
            get => _sourceFilePath;
            set
            {
                _sourceFilePath = value;
                NotifyPropChanged(nameof(SourceFilePath));
            }
        }

        private string _destFilePath;
        public string DestFilePath
        {
            get => _destFilePath;
            set
            {
                _destFilePath = value;
                NotifyPropChanged(nameof(DestFilePath));
            }
        }

        private string _statusString = "aga";
        public string StatusString
        {
            get => _statusString;
            set
            {
                _statusString = value;
                NotifyPropChanged(nameof(StatusString));
            }
        }

        private double _cryptoProgress = 0;
        public double CryptoProgress
        {
            get => _cryptoProgress;
            set
            {
                if (value > 100)
                    _cryptoProgress = 100;
                else if (value < 0)
                    _cryptoProgress = 0;
                else
                    _cryptoProgress = value;
                NotifyPropChanged(nameof(CryptoProgress));
            }
        }

        private bool _isDone = false;
        public bool IsDone
        {
            get => _isDone;
            set
            {
                _isDone = value;
                NotifyPropChanged(nameof(IsDone));
            }
        }

        private string _cryptoName = "";
        public string CryptoName
        {
            get => _cryptoName;
            set
            {
                _cryptoName = value;
                NotifyPropChanged(nameof(CryptoName));
            }
        }

        private RelayCommand _cancelCmd;
        public RelayCommand CancelCmd
            => _cancelCmd ?? (_cancelCmd = new RelayCommand(_ => Cancel()));

        #endregion

        private bool _isDeleteAfter;
        private CryptoDirection? _direction;

        public BaseTransformVM(bool isDeleteAfter, CryptoDirection? direction)
        {
            _isDeleteAfter = isDeleteAfter;
            _direction = direction;
        }

        protected async void Start(ICryptoTransform transform)
        {
            if (_direction is null)
                StatusString = "Cryption...";
            else if (_direction == CryptoDirection.Encrypt)
                StatusString = "Encryption...";
            else
                StatusString = "Decryption...";

            try
            {
                await MakeTransform(transform);
                await DeleteSourceIfNeeded();
                OnDoneSuccessfully();
            }
            catch (OperationCanceledException)
            {
                OnCanceled();
            }
            catch (Exception e)
            {
                OnError(e.Message);
            }
        }

        private async Task MakeTransform(ICryptoTransform transform)
        {
            OperationCanceledException canceledException = null;

            try
            {
                using (FileStream inStream = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read))
                using (FileStream outStream = new FileStream(DestFilePath, FileMode.Create, FileAccess.Write))
                using (CryptoStream outCrypto = new CryptoStream(outStream, transform, CryptoStreamMode.Write))
                {
                    try
                    {
                        await inStream.CopyToAsync(outCrypto, 80_000, _cts.Token,
                            progress => CryptoProgress = progress);
                    }
                    catch (OperationCanceledException e)
                    {
                        canceledException = e;
                    }
                }
            }
            catch (Exception e)
            {
                if (canceledException is null)
                    throw e;
            }

            if (canceledException is object)
                throw canceledException;
        }

        protected async void StartMultithread(INiceCryptoTransform transform)
        {
            try
            {
                StatusString = "Reading file...";
                byte[] text = await File.ReadAllBytesAsync(SourceFilePath, _cts.Token);

                if (_direction is null)
                    StatusString = "Cryption...";
                else if (_direction == CryptoDirection.Encrypt)
                    StatusString = "Encryption...";
                else
                    StatusString = "Decryption...";
                byte[] transformed = await ECB.TransformAsync(text, transform, _cts.Token, 4,
                    progress => CryptoProgress = progress);

                StatusString = "Saving to file...";
                await File.WriteAllBytesAsync(DestFilePath, transformed, _cts.Token);

                await DeleteSourceIfNeeded();

                OnDoneSuccessfully();
            }
            catch (OperationCanceledException)
            {
                OnCanceled();
                return;
            }
            catch (Exception e)
            {
                OnError(e.Message);
                return;
            }
        }

        private async Task DeleteSourceIfNeeded()
        {
            if (_isDeleteAfter)
            {
                StatusString = "Deleting file...";
                await Task.Run(() => File.Delete(SourceFilePath));
            }
        }

        private void Cancel()
        {
            _cts.Cancel();
        }

        private void OnCanceled()
        {
            Reject();
            StatusString = "Canceled";
            IsDone = true;
        }

        private void OnError(string msg)
        {
            Reject();
            StatusString = "Error: " + msg;
            IsDone = true;
        }

        private void OnDoneSuccessfully()
        {
            StatusString = "Done successfully";
            IsDone = true;
        }

        protected virtual void Reject()
        {
            try
            {
                if (File.Exists(DestFilePath))
                    File.Delete(DestFilePath);
            }
            catch { }
        }

    }
}
