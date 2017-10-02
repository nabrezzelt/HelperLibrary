using System;

namespace HelperLibrary.Networking.ClientServer
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public BaseClientData Client;

        public ClientConnectedEventArgs(BaseClientData client)
        {
            Client = client;
        }
    }
}