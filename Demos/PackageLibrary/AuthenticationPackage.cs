using System;
using HelperLibrary.Networking.ClientServer;

namespace PackageLibrary
{
    [Serializable]
    public class AuthenticationPackage : BasePackage
    {        
        public AuthenticationPackage(string senderUid, string destinationUid) : base(senderUid, destinationUid) { }        
    }
}
