using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vTalkServer.tools
{
    public class PacketWriter
    {

        private byte[] Buffer { get; set; }
        public int Length { get; private set; }
        public int Position { get; set; }

        public PacketWriter()
        {
            Buffer = new byte[0x50];
        }

        private void EnsureCapacity(int length)
        {
            if (Position + length < Buffer.Length) return;
            byte[] newBuffer = new byte[Buffer.Length + 0x50];
            System.Buffer.BlockCopy(Buffer, 0, newBuffer, 0, Buffer.Length);
            Buffer = newBuffer;
            EnsureCapacity(length);
        }

        public void WriteBytes(byte[] bytes)
        {
            int length = bytes.Length;
            if (bytes == null || length == 0)
                throw new ArgumentNullException("bytes", "Trying to write zero or null bytes");

            EnsureCapacity(length);
            System.Buffer.BlockCopy(bytes, 0, Buffer, Position, length);

            Length += length;
            Position += length;
        }

        public void WriteBool(bool value)
        {
            WriteByte((byte)(value ? 1 : 0));
        }

        public void WriteSByte(sbyte value)
        {
            sbyte[] signed = { value };
            WriteBytes((byte[])(Array)signed);
        }

        public void WriteByte(byte value)
        {
            WriteBytes(new byte[1] { value });
        }

        public void WriteShort(short value)
        {
            WriteBytes(new byte[2] {
                (byte)value,
                (byte)(value >> 8)
            });
        }

        public void WriteUShort(ushort value)
        {
            WriteBytes(new byte[2] {
                (byte)value,
                (byte)(value >> 8)
            });
        }

        public void WriteInt(int value)
        {
            WriteBytes(new byte[4] {
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24)
            });
        }

        public void WriteUInt(uint value)
        {
            WriteBytes(new byte[4] {
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24)
            });
        }

        public void WriteLong(long value)
        {
            WriteBytes(new byte[8] {
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                (byte)(value >> 32),
                (byte)(value >> 40),
                (byte)(value >> 48),
                (byte)(value >> 56)
            });
        }

        public void WriteULong(ulong value)
        {
            WriteBytes(new byte[8] {
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                (byte)(value >> 32),
                (byte)(value >> 40),
                (byte)(value >> 48),
                (byte)(value >> 56)
            });
        }

        public void WriteString(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            WriteInt(bytes.Length);
            WriteBytes(bytes);
        }

        public byte[] ToArray()
        {
            byte[] toRet = new byte[Length];
            System.Buffer.BlockCopy(Buffer, 0, toRet, 0, Length);
            return toRet;
        }
    }
}
