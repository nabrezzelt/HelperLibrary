using HelperLibrary.Networking.ClientServer.Packages;
using System;

namespace PackageLibrary
{
    [Serializable]
    public class AuthenticationPackage : BasePackage
    {        
        public AuthenticationPackage(string senderUid, string destinationUid) : base(senderUid, destinationUid) { }        
    }
}
