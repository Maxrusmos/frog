using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CryptographyLabs.Crypto.IO
{
    public class NiceCryptoReadStream : Stream
    {
        private Stream _source;
        private INiceCryptoTransform _transform;

        private bool _isEOS = false;
        private int _bufBlocksCapacity = 6_000;
        private int _bytesInInput = 0;
        private byte[] _inputBuffer;
        private int _bytesInOutput = 0;
        private byte[] _outputBuffer;

        public NiceCryptoReadStream(Stream source, INiceCryptoTransform transform)
        {
            _source = source;
            _transform = transform;

            _inputBuffer = new byte[_bufBlocksCapacity * transform.InputBlockSize];
            _outputBuffer = new byte[_bufBlocksCapacity * transform.OutputBlockSize];
        }

        #region Stream

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        /// <exception cref="NotSupportedException"></exception>
        public override long Length => throw new NotSupportedException();
        /// <exception cref="NotSupportedException"></exception>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                _source.Dispose();
        }

        public override void Flush()
        {

        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count <= _bytesInOutput)
            {
                Array.Copy(_outputBuffer, 0, buffer, offset, count);
                _bytesInOutput -= count;
                Array.Copy(_outputBuffer, count, _outputBuffer, 0, _bytesInOutput);
                return count;
            }
            else// count > _bytesInOutput
            {
                Array.Copy(_outputBuffer, 0, buffer, offset, _bytesInOutput);
                offset += _bytesInOutput;
                int hasSent = _bytesInOutput;
                _bytesInOutput = 0;

                while (hasSent < count)
                {
                    if (_isEOS)
                        break;

                    int bytesNeed = count - hasSent;
                    int blocksNeed = bytesNeed / _transform.OutputBlockSize;
                    if (bytesNeed % _transform.OutputBlockSize != 0)
                        blocksNeed += 1;

                    FillInputBuffer(blocksNeed);

                    Transform();

                    if (bytesNeed <= _bytesInOutput)
                    {
                        Array.Copy(_outputBuffer, 0, buffer, offset, bytesNeed);
                        offset += bytesNeed;
                        Array.Copy(_outputBuffer, bytesNeed, _outputBuffer, 0, _bytesInOutput - bytesNeed);
                        _bytesInOutput -= bytesNeed;
                        hasSent += bytesNeed;
                    }
                    else// bytesNeed > _bytesInOutput
                    {
                        Array.Copy(_outputBuffer, 0, buffer, offset, _bytesInOutput);
                        offset += _bytesInOutput;
                        hasSent += _bytesInOutput;
                        _bytesInOutput = 0;
                    }
                }
                return hasSent;
            }
        }

        /// <exception cref="NotSupportedException"></exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <exception cref="NotSupportedException"></exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <exception cref="NotSupportedException"></exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #endregion

        private void FillInputBuffer(int blocksNeed)
        {
            if (blocksNeed >= _bufBlocksCapacity - 1)
            {
                FillInputBuffer();
                return;
            }

            int bytesNeed = (blocksNeed + 1) * _transform.InputBlockSize;
            while (_bytesInInput < bytesNeed)
            {
                int hasRead = _source.Read(_inputBuffer, _bytesInInput, bytesNeed - _bytesInInput);
                if (hasRead == 0)
                {
                    _isEOS = true;
                    break;
                }
                _bytesInInput += hasRead;
            }
        }

        private void FillInputBuffer()
        {
            while (_bytesInInput < _inputBuffer.Length)
            {
                int hasRead = _source.Read(_inputBuffer, _bytesInInput, _inputBuffer.Length - _bytesInInput);
                if (hasRead == 0)
                {
                    _isEOS = true;
                    break;
                }
                _bytesInInput += hasRead;
            }
        }

        private void Transform()
        {
            int fullBlocksCount = _bytesInInput / _transform.InputBlockSize;
            int finalBytesCount = _bytesInInput % _transform.InputBlockSize;

            if (finalBytesCount == 0)
            {
                _transform.NiceTransform(_inputBuffer, 0, _outputBuffer, 0, fullBlocksCount - 1);
                _bytesInOutput = (fullBlocksCount - 1) * _transform.OutputBlockSize;
            }
            else
            {
                _transform.NiceTransform(_inputBuffer, 0, _outputBuffer, 0, fullBlocksCount);
                _bytesInOutput = fullBlocksCount * _transform.OutputBlockSize;
            }

            if (_isEOS)
            {
                byte[] final = finalBytesCount switch
                {
                    0 => _transform.NiceFinalTransform(_inputBuffer,
                            (fullBlocksCount - 1) * _transform.InputBlockSize,
                            _transform.InputBlockSize),
                    _ => _transform.NiceFinalTransform(_inputBuffer,
                            fullBlocksCount * _transform.InputBlockSize,
                            finalBytesCount)
                };
                Array.Copy(final, 0, _outputBuffer, _bytesInOutput, final.Length);
                _bytesInOutput += final.Length;
                _bytesInInput = 0;
            }
            else
            {
                if (finalBytesCount == 0)
                {
                    Array.Copy(_inputBuffer, (fullBlocksCount - 1) * _transform.InputBlockSize,
                        _inputBuffer, 0, _transform.InputBlockSize);
                    _bytesInInput = _transform.InputBlockSize;
                }
                else
                {
                    Array.Copy(_inputBuffer, fullBlocksCount * _transform.InputBlockSize,
                        _inputBuffer, 0, finalBytesCount);
                    _bytesInInput = finalBytesCount;
                }
            }
        }
    }
}
