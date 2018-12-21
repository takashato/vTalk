using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vTalkServer.tools
{
    class PacketReader
    {
        protected byte[] Buffer { get; private set; }
        public int Length { get; private set; }
        public int Position { get; set; }
        public int Available
        {
            get { return Length - Position; }
        }

        public PacketReader(byte[] data)
        {
            Length = data.Length;
            Buffer = new byte[Length];
            System.Buffer.BlockCopy(data, 0, Buffer, 0, Length);
        }

        private int StartRead(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException("length", "Length cannot be zero or negative");

            int sPosition = Position;
            Position += length;
            if (Available < 0)
            {
                Position = sPosition;
                throw new Exception("Not enough data");
            }

            return sPosition;
        }

        public bool ReadBool()
        {
            return Buffer[StartRead(1)] > 0;
        }

        public sbyte ReadSByte()
        {
            return (sbyte)Buffer[StartRead(1)];
        }

        public byte ReadByte()
        {
            return Buffer[StartRead(1)];
        }

        public byte[] ReadBytes(int length)
        {
            byte[] toRead = new byte[length];
            System.Buffer.BlockCopy(Buffer, StartRead(length), toRead, 0, length);
            return toRead;
        }

        public short ReadShort()
        {
            return BitConverter.ToInt16(Buffer, StartRead(2));
        }

        public ushort ReadUShort()
        {
            return BitConverter.ToUInt16(this.Buffer, StartRead(2));
        }

        public int ReadInt()
        {
            return BitConverter.ToInt32(this.Buffer, StartRead(4));
        }

        public uint ReadUInt()
        {
            return BitConverter.ToUInt32(this.Buffer, StartRead(4));
        }

        public long ReadLong()
        {
            return BitConverter.ToInt64(this.Buffer, StartRead(8));
        }

        public ulong ReadULong()
        {
            return BitConverter.ToUInt64(this.Buffer, StartRead(8));
        }

        public string ReadString()
        {
            int length = ReadInt();
            if (length == 0) return string.Empty;
            byte[] bytes = ReadBytes(length);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
    }
}
