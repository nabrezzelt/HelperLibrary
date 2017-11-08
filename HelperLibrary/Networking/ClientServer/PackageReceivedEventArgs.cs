using System;
using System.Net.Sockets;

namespace HelperLibrary.Networking.ClientServer
{
    public class PackageReceivedEventArgs : EventArgs
    {
        public object Package;
        public TcpClient SenderTcpClient;

        public PackageReceivedEventArgs(object package, TcpClient senderTcpClient)
        {
            Package = package;
            SenderTcpClient = senderTcpClient;
        }
    }
}