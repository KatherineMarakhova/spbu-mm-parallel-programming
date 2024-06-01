using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace ChatApp
{

    public partial class Chat
    {
        private delegate void SafeCallDelegate(string text);
        private Socket BroadcastSendSocket;
        private Socket BroadcastRecvSocket;
        private IPEndPoint RemoteEndPoint = new IPEndPoint(new IPAddress(new byte[] {255, 255, 255, 255}), 1000);
        private Thread Listener;

        public void Connect(int userId, CancellationToken token)
        {
            try
            {
                BroadcastSendSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Dgram, ProtocolType.Udp);
                BroadcastSendSocket.SetSocketOption(SocketOptionLevel.Socket,
                    SocketOptionName.Broadcast, true);
                BroadcastSendSocket.SetSocketOption(SocketOptionLevel.Socket,
                    SocketOptionName.ReuseAddress, true);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message + '\n');
                return;
            }
            Send(userId, "was conneted!\n");

            Listener = new Thread(() => Listen(token));
            Listener.Start();
        }

        private void Listen(CancellationToken token)
        {
            try
            {
                BroadcastRecvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                BroadcastRecvSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                BroadcastRecvSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                var broadcastEndpoint = new IPEndPoint(IPAddress.Any, 1000);
                BroadcastRecvSocket.Bind(broadcastEndpoint);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message + " - Error with socket creation\n");
                return;
            }
            while (!token.IsCancellationRequested)
            {
                if (BroadcastRecvSocket.Available != 0)
                {
                    try
                    {
                        byte[] b = new byte[1024];
                        BroadcastRecvSocket.Receive(b);
                        Console.WriteLine(Encoding.ASCII.GetString(b, 0, Array.IndexOf(b, (byte)0)) + '\n');
                    }
                    catch (SocketException se)
                    {
                        Console.WriteLine(se.Message + " - Error with receiving\n");
                        return;
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }
                }
                Thread.Yield();
            }
        }

        public void Send(int userId, string message)
        {
            try
            {
                byte[] byteMessage = Encoding.ASCII.GetBytes(userId + " : " + message + '\n');
                BroadcastSendSocket.SendTo(byteMessage, RemoteEndPoint);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message + " - Error with sending message\n");
                return;
            }
        }
    }
}
