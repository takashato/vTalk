﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using vTalkServer.constants;
using vTalkServer.tools;

namespace vTalkServer.server
{
    class Client
    {
        public ClientConnection Connection { get; set; }

        public bool Connected { get; set; }

        public IPEndPoint IPEndPoint { get; set; }

        public Client(Socket session)
        {
            this.Connection = new ClientConnection(this, session);
            IPEndPoint = Connection.IPEndPoint;
            Connected = true;
        }

        public void Disconnect(string reason, params object[] values)
        {
            Console.WriteLine("Ngắt kết nối client: " + string.Format(reason, values));
            if (Connection != null)
                Connection.Disconnect();
        }

        internal void Disconnected()
        {
            try
            {
                if (Connected)
                {
                    Console.WriteLine(string.Format("{0}:{1} đã ngắt kết nối.", Connection.Host, Connection.Port));
                }
                Connected = false;

                /*
                Program.Users.Remove(IPEndPoint.ToString());
                ServerForm.Instance.RemoveClient(this);
                ServerForm.Instance.UpdateUserList();*/
                Connection.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Gặp lỗi khi ngắt kết nối client. " + e.ToString());
            }
        }

        internal void RecvPacket(byte[] rawData)
        {
            PacketReader pr_raw = new PacketReader(rawData);
            int packetLength = pr_raw.ReadInt();
            int dataType = pr_raw.ReadByte();
            byte[] data = new byte[packetLength];
            Buffer.BlockCopy(rawData, PacketProcessor.HeaderSize, data, 0, packetLength);
            switch ((RecvHeader)dataType)
            {

            }
        }
    }
}