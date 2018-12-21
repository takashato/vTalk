using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using vTalkServer.tools;

namespace vTalkServer.server
{
    class ClientConnection : IDisposable
    {
        private readonly Client client;
        private readonly Socket socket;
        private readonly byte[] socketBuffer;
        private readonly string host;
        private readonly int port;

        private bool disposed;
        private object disposeSync;

        private PacketProcessor packetProcessor;

        public bool Connected
        {
            get
            {
                return disposed == false;
            }
        }

        public string Host
        {
            get
            {
                return host;
            }
        }

        public int Port
        {
            get
            {
                return port;
            }
        }

        public IPEndPoint IPEndPoint
        {
            get
            {
                return (IPEndPoint)socket.RemoteEndPoint;
            }
        }

        public ClientConnection(Client client, Socket socket)
        {
            this.client = client;
            this.socket = socket;
            socketBuffer = new byte[1024];

            host = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            port = ((IPEndPoint)socket.RemoteEndPoint).Port;

            disposeSync = new object();

            packetProcessor = new PacketProcessor();
            packetProcessor.PacketFinished += (data) => // Handle PacketFinished event
            {
                client.RecvPacket(data);
            };

            WaitForData();
        }

        private void WaitForData()
        {
            if (!disposed)
            {
                SocketError error = SocketError.Success;
                socket.BeginReceive(socketBuffer, 0, socketBuffer.Length, SocketFlags.None, out error, OnPacketReceived, null);

                if (error != SocketError.Success)
                {
                    Disconnect();
                }
            }
        }

        private void OnPacketReceived(IAsyncResult iar)
        {
            if (!disposed)
            {
                SocketError error = SocketError.Success;
                int size = socket.EndReceive(iar, out error);

                if (size == 0 || error != SocketError.Success)
                {
                    Disconnect();
                }
                else
                {
                    packetProcessor.AddData(socketBuffer, 0, size); // Add data to process
                    WaitForData(); // Wait for data again
                }
            }
        }

        public void Disconnect()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (disposeSync)
            {
                if (disposed == true)
                    return;

                disposed = true;

                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                finally
                {
                    client.Disconnected();
                }
            }
        }

        ~ClientConnection()
        {
            Dispose();
        }

        public void SendData(SendHeader dataType, byte[] pData)
        {
            byte[] data = new byte[pData.Length + PacketProcessor.HeaderSize];
            PacketWriter pw = new PacketWriter();
            pw.WriteInt(pData.Length);
            pw.WriteByte((byte)dataType);
            Buffer.BlockCopy(pw.ToArray(), 0, data, 0, pw.Length);
            Buffer.BlockCopy(pData, 0, data, PacketProcessor.HeaderSize, pData.Length);
            pData = data;
            try
            {
                SendPacket(pData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Gửi thất bại: {0}", ex.ToString());
            }
        }

        public void SendPacket(byte[] final)
        {
            if (!disposed)
            {
                int offset = 0;

                while (offset < final.Length)
                {
                    SocketError outError = SocketError.Success;
                    int sent = socket.Send(final, offset, final.Length - offset, SocketFlags.None, out outError);

                    if (sent == 0 || outError != SocketError.Success)
                    {
                        Disconnect();
                        return;
                    }

                    offset += sent;
                }
            }
        }
    }
}
