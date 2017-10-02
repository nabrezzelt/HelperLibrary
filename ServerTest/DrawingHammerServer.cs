using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using HelperLibrary.Networking.ClientServer;

namespace ServerDemo
{
    class DrawingHammerServer : Server
    {
        public DrawingHammerServer(int port) : base(port) { }

        protected override BaseClientData HandleNewConnectedClient(TcpClient connectedClient)
        {
            //Um eigene Werte hinzuzufügen (Score) wir die Standard Methode überschrieben
            return new DrawingHammerClient(this, connectedClient);
        }
    }
}
