using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabs.Crypto
{
    public static partial class DES_
    {
        public const int BlockSize = 8;

        public class DESEncryptTransform : INiceCryptoTransform, ICryptoTransform
        {
            private ulong[] _keys48;

            public DESEncryptTransform(ulong key56)
            {
                _keys48 = GenerateKeys(key56);
            }

            #region ICryptoTransform interface

            public int InputBlockSize => BlockSize;
            public int OutputBlockSize => BlockSize;
            public bool CanTransformMultipleBlocks => true;
            public bool CanReuseTransform => false;

            public void Dispose() { }

            public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
            {
                int blocksCount = inputCount / BlockSize;
                NiceTransform(inputBuffer, inputOffset, outputBuffer, outputOffset, blocksCount);
                return blocksCount * BlockSize;
            }

            public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
            {
                return NiceFinalTransform(inputBuffer, inputOffset, inputCount);
            }

            #endregion

            #region INiceCryptoTransform interface

            public void NiceTransform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset, int blocksCount)
            {
                for (int i = 0; i < blocksCount; ++i)
                {
                    ulong text = BitConverter.ToUInt64(inputBuffer, inputOffset + i * BlockSize);
                    ulong result = Encrypt(text, _keys48);
                    Array.Copy(BitConverter.GetBytes(result), 0, outputBuffer, outputOffset + i * BlockSize, BlockSize);
                }
            }

            public byte[] NiceFinalTransform(byte[] inputBuffer, int inputOffset, int bytesCount)
            {
                if (bytesCount == BlockSize)
                {
                    byte[] tm = new byte[2 * BlockSize];
                    Array.Copy(inputBuffer, inputOffset, tm, 0, BlockSize);
                    tm[2 * BlockSize - 1] = 0;
                    byte[] final = new byte[2 * BlockSize];
                    NiceTransform(tm, 0, final, 0, 2);
                    return final;
                }
                else
                {
                    byte[] tm = new byte[BlockSize];
                    Array.Copy(inputBuffer, inputOffset, tm, 0, bytesCount);
                    tm[BlockSize - 1] = (byte)bytesCount;
                    byte[] final = new byte[BlockSize];
                    NiceTransform(tm, 0, final, 0, 1);
                    return final;
                }
            }

            #endregion
        }

        public class DESDecryptTransform : INiceCryptoTransform, ICryptoTransform
        {
            private ulong[] _keys48;
            private bool _isFirst = true;
            private byte[] _lastBlock = new byte[BlockSize];

            public DESDecryptTransform(ulong key56)
            {
                _keys48 = GenerateKeys(key56);
            }

            #region ICryptoTransform interface

            public int InputBlockSize => BlockSize;
            public int OutputBlockSize => BlockSize;
            public bool CanTransformMultipleBlocks => true;
            public bool CanReuseTransform => false;

            public void Dispose() { }

            public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
            {
                int blocksCount = inputCount / BlockSize;
                int offset = _isFirst ? 0 : BlockSize;
                if (!_isFirst)
                {
                    ulong text = BitConverter.ToUInt64(_lastBlock, 0);
                    ulong result = Decrypt(text, _keys48);
                    Array.Copy(BitConverter.GetBytes(result), 0, outputBuffer, outputOffset, BlockSize);
                }

                for (int i = 0; i < blocksCount - 1; ++i)
                {
                    ulong text = BitConverter.ToUInt64(inputBuffer, inputOffset + i * BlockSize);
                    ulong result = Decrypt(text, _keys48);
                    Array.Copy(BitConverter.GetBytes(result), 0,
                        outputBuffer, outputOffset + i * BlockSize + offset, BlockSize);
                }
                Array.Copy(inputBuffer, inputOffset + blocksCount * BlockSize - BlockSize, _lastBlock, 0, BlockSize);

                if (_isFirst)
                {
                    _isFirst = false;
                    return (blocksCount - 1) * BlockSize;
                }
                else
                    return blocksCount * BlockSize;
            }

            public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
            {
                if (inputCount != 0)
                    throw new CryptographicException("Wrong length of final block on decryption.");

                if (_isFirst)
                    throw new CryptographicException("Wrong count of blocks.");

                ulong text = BitConverter.ToUInt64(_lastBlock, 0);
                ulong decryptedText = Decrypt(text, _keys48);
                byte[] decrypted = BitConverter.GetBytes(decryptedText);

                byte bytesCount = decrypted[BlockSize - 1];
                byte[] result = new byte[bytesCount];
                Array.Copy(decrypted, result, bytesCount);
                return result;
            }

            #endregion

            #region INiceCryptoTransform interface

            public void NiceTransform(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset, int blocksCount)
            {
                for (int i = 0; i < blocksCount; ++i)
                {
                    ulong text = BitConverter.ToUInt64(inputBuffer, inputOffset + i * BlockSize);
                    ulong result = Decrypt(text, _keys48);
                    Array.Copy(BitConverter.GetBytes(result), 0,
                        outputBuffer, outputOffset + i * BlockSize, BlockSize);
                }
            }

            public byte[] NiceFinalTransform(byte[] inputBuffer, int inputOffset, int bytesCount)
            {
                if (bytesCount != BlockSize)
                    throw new CryptographicException("Wrong length of final block on NICE decryption.");

                byte[] final = new byte[BlockSize];
                NiceTransform(inputBuffer, inputOffset, final, 0, 1);
                Array.Resize(ref final, final[BlockSize - 1]);
                return final;
            }

            #endregion
        }

        private static ulong[] GenerateKeys(ulong baseKey)
        {
            ulong key64 = 0;
            for (int i = 0; i < 8; ++i)
            {
                byte bitSequence = (byte)((baseKey >> (i * 7)) & 0b01111111);
                byte xorRes = Bitops.XorBits(bitSequence, 3);
                key64 = (key64 << 1) | (byte)(~xorRes & 1);
                key64 = (key64 << 7) | bitSequence;
            }

            uint C = (uint)(Bitops.SwapBitsMasks64(key64, _C0PermMasks) & 0xf_ff_ff_ff);
            uint D = (uint)(Bitops.SwapBitsMasks64(key64, _D0PermMasks) & 0xf_ff_ff_ff);

            ulong[] keys = new ulong[16];
            for (int i = 0; i < 16; ++i)
            {
                C = Bitops.CycleShiftLeft(C, 28, _cycleShiftsCount[i]);
                D = Bitops.CycleShiftLeft(D, 28, _cycleShiftsCount[i]);
                keys[i] = ((ulong)C << 28) | D;
                keys[i] = Bitops.SwapBitsMasks64(keys[i], _keyFinalPermMasks) & 0xff_ff_ff_ff_ff_ff;
            }

            return keys;
        }

        private static ulong Encrypt(ulong text, ulong[] keys48)
        {
            text = Bitops.SwapBitsMasks64(text, _IPPermMasks);
            uint L = (uint)(text >> 32);
            uint R = (uint)(text & 0xffffffff);

            for (int i = 0; i < 16; ++i)
            {
                uint tm = L;
                L = R;
                R = tm ^ FeistelFunction(R, keys48[i]);
            }

            ulong concat = ((ulong)L << 32) | R;
            return Bitops.SwapBitsMasks64(concat, _IPInvPermMasks);
        }

        private static ulong Decrypt(ulong crText, ulong[] keys48)
        {
            crText = Bitops.SwapBitsMasks64(crText, _IPPermMasks);
            uint L = (uint)(crText >> 32);
            uint R = (uint)(crText & 0xffffffff);

            for (int i = 15; i > -1; --i)
            {
                uint tm = R;
                R = L;
                L = tm ^ FeistelFunction(L, keys48[i]);
            }

            ulong concat = ((ulong)L << 32) | R;
            return Bitops.SwapBitsMasks64(concat, _IPInvPermMasks);
        }

        private static uint FeistelFunction(uint value, ulong key48)
        {
            ulong value48 = EExpansion(value) ^ key48;

            uint SBlocksResult = 0;
            for (int i = 7; i > -1; --i)
            {
                byte row = (byte)(((value48 >> (i * 6 + 4)) & 0b10) | ((value48 >> (i * 6)) & 1));
                byte col = (byte)((value48 >> (i * 6 + 1)) & 0b1111);
                SBlocksResult = (SBlocksResult << 4) | _SBlocks[8 - i - 1][row][col];
            }

            return (uint)Bitops.SwapBitsMasks64(SBlocksResult, _PPermMasks);
        }

        private static ulong EExpansion(uint value)
        {
            ulong result = 0;
            for (int i = 0; i < 8; ++i)
                result = (result << 6) | (Bitops.CycleShiftLeft(value, 32, (byte)(5 + i * 4)) & 0b111111);
            return result;
        }
    }
}
