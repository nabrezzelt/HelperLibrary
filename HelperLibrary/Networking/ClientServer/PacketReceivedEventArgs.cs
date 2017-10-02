using System;
using System.Net.Sockets;

namespace HelperLibrary.Networking.ClientServer
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public object Packet;
        public TcpClient SenderTcpClient;

        public PacketReceivedEventArgs(object packet, TcpClient senderTcpClient)
        {
            Packet = packet;
            SenderTcpClient = senderTcpClient;
        }
    }
}