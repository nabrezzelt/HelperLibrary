using System;
using System.Net;
using HelperLibrary.Networking.ClientServer.Packets;

namespace HelperLibrary.Networking.ClientServer
{
    public interface IClient
    {
        event EventHandler ConnectionSucceed;
        event EventHandler ConnectionLost;

        event EventHandler<PacketReceivedEventArgs> PacketReceived;

        void Connect(IPAddress serverIP, int port);

        void Disconnect();

        void SendPacketToServer(BasePacket packet);
    }
}