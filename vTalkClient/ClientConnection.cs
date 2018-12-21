using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using vTalkClient.tools;

namespace vTalkClient
{
    public class ClientConnection : IDisposable
    {
        private readonly Socket socket;
        private readonly byte[] socketBuffer;
        private readonly string host;
        private readonly int port;
        private readonly object dispose_Sync;
        private bool disposed;
        public PacketProcessor PProcessor { get; private set; }

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

        public ClientConnection(Socket socket)
        {
            this.socket = socket;
            socketBuffer = new byte[1024];

            host = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            port = ((IPEndPoint)socket.RemoteEndPoint).Port;

            dispose_Sync = new object();


            PProcessor = new PacketProcessor();
            PProcessor.PacketFinished += (data) =>
            {
                RecvPacket(data);
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
                    PProcessor.AddData(socketBuffer, 0, size);
                    WaitForData();
                }
            }
        }

        public void Disconnect()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (dispose_Sync)
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
                    //ClientForm.Instance.EnableChat(false);

                    //ClientForm.Instance.UserList = new Dictionary<string, string>();
                    Console.WriteLine("Bạn đã bị ngắt kết nối với server.");
                    /*try
                    {
                        Console.WriteLine(string.Format("{0}:{1} Disconnected", SClient.Host, SClient.Port));
                        Program.Users.Remove(IPEndPoint.ToString());
                        MainForm.Instance.RemoveClient(this);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: Error when disconneting client. " + e.ToString());
                    }*/
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

        private void SendPacket(byte[] data)
        {
            if (!disposed)
            {
                int offset = 0;

                while (offset < data.Length)
                {
                    SocketError outError = SocketError.Success;
                    int sent = socket.Send(data, offset, data.Length - offset, SocketFlags.None, out outError);

                    if (sent == 0 || outError != SocketError.Success)
                    {
                        Disconnect();
                        return;
                    }
                    offset += sent;
                }
            }
        }

        internal void RecvPacket(byte[] packet)
        {
            PacketReader pr_raw = new PacketReader(packet);
            int packetLength = pr_raw.ReadInt();
            int dataType = pr_raw.ReadByte();
            byte[] data = new byte[packetLength];
            Buffer.BlockCopy(packet, PacketProcessor.HeaderSize, data, 0, packetLength);
            switch ((RecvHeader)dataType)
            {
                case RecvHeader.LoginResult:
                    PacketReader pr = new PacketReader(data);
                    LoginStatus status = (LoginStatus)pr.ReadByte();
                    if(status == LoginStatus.Success)
                    {
                        ClientWindow.Instance.AccountInfo = new account.AccountInfo();
                        ClientWindow.Instance.AccountInfo.Decode(pr);
                        ClientWindow.Instance.CloseLogin();
                    }
                    break;
            }
        }
    }
}
