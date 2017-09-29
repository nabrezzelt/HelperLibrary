using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace HelperLibrary.Networking.ClientServer
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public object Packet;

        public PacketReceivedEventArgs(object packet)
        {
            Packet = packet;
        }
    }
}