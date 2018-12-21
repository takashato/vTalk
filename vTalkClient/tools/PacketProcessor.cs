using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vTalkServer.tools
{
    class PacketProcessor
    {
        /// <summary>
        /// Packet's Header Size, include Packet Length (int32) and header (int8)
        /// </summary>
        public static int HeaderSize = 5;

        /// <summary>
        /// Waiting state
        /// </summary>
        private bool IsWaiting { get; set; }

        /// <summary>
        /// Data buffer
        /// </summary>
        private byte[] DataBuffer { get; set; }

        /// <summary>
        /// Amount of data to wait on
        /// </summary>
        private int WaitForData { get; set; }

        /// <summary>
        /// Callback for when a packet is finished
        /// </summary>
        public delegate void CallPacketFinished(byte[] packet);

        /// <summary>
        /// Event called when a packet has been handled
        /// </summary>
        public event CallPacketFinished PacketFinished;

        /// <summary>
        /// Create new instance of <see cref="PacketProcessor"/>
        /// </summary>
        public PacketProcessor()
        {
            DataBuffer = new byte[0x100];
            WaitForData = 0;
            IsWaiting = true;
        }

        /// <summary>
        /// Add binary data to the buffer
        /// </summary>
        /// <param name="data">data buffer to add</param>
        /// <param name="offset">data offset</param>
        /// <param name="length">data length</param>
        public void AddData(byte[] data, int offset, int length)
        {
            if (IsWaiting)
            {
                if(length < HeaderSize) // Packet error, not enough header byte
                {
                    return;
                }

                PacketReader pr = new PacketReader(data);
                int packetLength = pr.ReadInt(); // Get packet length 
                DataBuffer = new byte[packetLength + HeaderSize]; // Create new buffer with length = packetLength + HeaderSize
                Buffer.BlockCopy(data, 0, DataBuffer, 0, length); // Copy data to general buffer
                if (length < packetLength + HeaderSize) // If packet length including header size is smaller than actual size, wait for the remaining
                {
                    WaitForData = packetLength + HeaderSize - length;
                    IsWaiting = false;
                }
                else // Finish processing packet
                {
                    PacketFinished?.Invoke(DataBuffer); // Invoke Finish handler
                }
            }
            else // Remaining received
            {
                Buffer.BlockCopy(data, 0, DataBuffer, DataBuffer.Length - WaitForData, length); // Add data in this buffer to general buffer
                WaitForData -= length; // Decrease data amount to be waited
                if (WaitForData <= 0) // Received all data
                {
                    if (PacketFinished != null)
                        PacketFinished(DataBuffer);
                    IsWaiting = true; // Reset processor
                }
            }
        }
    }
}
