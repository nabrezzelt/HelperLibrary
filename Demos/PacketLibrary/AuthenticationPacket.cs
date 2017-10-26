using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace PacketLibrary
{
    [Serializable]
    public class AuthenticationPacket : BasePacket
    {        
        public AuthenticationPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid) { }        
    }
}
