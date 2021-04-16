using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CryptographyLabs.Crypto.IO
{
    public class NiceCryptoWriteStream : Stream
    {
        private Stream _dest;
        private INiceCryptoTransform _transform;

        private int _bytesInIntermediate = 0;
        private byte[] _intermediateBuffer;
        private byte[] _outputBuffer;

        public NiceCryptoWriteStream(Stream dest, INiceCryptoTransform transform)
        {
            _dest = dest;
            _transform = transform;

            _intermediateBuffer = new byte[transform.InputBlockSize];
            _outputBuffer = new byte[transform.OutputBlockSize];
        }

        #region Stream

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
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
            Flush();
            base.Dispose(disposing);
            if (disposing)
                _dest.Dispose();
        }

        public override void Flush()
        {
            byte[] final = _transform.NiceFinalTransform(_intermediateBuffer, 0, _bytesInIntermediate);
            _dest.Write(final);
            _bytesInIntermediate = 0;
        }

        /// <exception cref="NotSupportedException"></exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
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

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count <= _intermediateBuffer.Length - _bytesInIntermediate)
            {
                Array.Copy(buffer, offset, _intermediateBuffer, _bytesInIntermediate, count);
                _bytesInIntermediate += count;
            }
            else// count > _intermediateBuffer.Length - _bytesInIntermediate
            {
                int bytesToCopy = _intermediateBuffer.Length - _bytesInIntermediate;
                Array.Copy(buffer, offset, _intermediateBuffer, _bytesInIntermediate, bytesToCopy);
                count -= bytesToCopy;
                offset += bytesToCopy;
                _transform.NiceTransform(_intermediateBuffer, 0, _outputBuffer, 0, 1);
                _dest.Write(_outputBuffer, 0, _transform.OutputBlockSize);

                int fullBlocksCount = count / _transform.InputBlockSize;
                int remains = count % _transform.InputBlockSize;
                if (remains == 0)
                    fullBlocksCount--;

                if (_outputBuffer.Length < fullBlocksCount * _transform.OutputBlockSize)
                    Array.Resize(ref _outputBuffer, fullBlocksCount * _transform.OutputBlockSize);

                _transform.NiceTransform(buffer, offset, _outputBuffer, 0, fullBlocksCount);
                _dest.Write(_outputBuffer, 0, fullBlocksCount * _transform.OutputBlockSize);

                if (remains == 0)
                {
                    Array.Copy(buffer, offset + fullBlocksCount * _transform.InputBlockSize,
                        _intermediateBuffer, 0, _transform.InputBlockSize);
                    _bytesInIntermediate = _transform.InputBlockSize;
                }
                else
                {
                    Array.Copy(buffer, offset + fullBlocksCount * _transform.InputBlockSize,
                        _intermediateBuffer, 0, remains);
                    _bytesInIntermediate = remains;
                }
            }
        }

        #endregion
    }
}
