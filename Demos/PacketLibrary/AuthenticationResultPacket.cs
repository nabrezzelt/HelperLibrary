using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace PacketLibrary
{
    [Serializable]
    public class AuthenticationResultPacket : BasePacket
    {
        public readonly AuthenticationResult Result;
        public AuthenticationResultPacket(AuthenticationResult result, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Result = result;
        }
    }
}
