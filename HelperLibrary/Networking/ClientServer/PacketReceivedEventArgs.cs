using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace HelperLibrary.Networking.ClientServer
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public BasePacket Packet;

        public PacketReceivedEventArgs(BasePacket packet)
        {
            Packet = packet;
        }
    }
}