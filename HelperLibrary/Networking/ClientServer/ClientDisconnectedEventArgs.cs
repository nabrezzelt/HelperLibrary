using System;

namespace HelperLibrary.Networking.ClientServer
{
    public class ClientDisconnectedEventArgs : EventArgs
    {
        public string ClientUid;

        public ClientDisconnectedEventArgs(string clientUid)
        {
            ClientUid = clientUid;
        }
    }
}