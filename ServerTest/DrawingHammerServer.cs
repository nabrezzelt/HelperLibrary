using HelperLibrary.Networking.ClientServer;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace ServerDemo
{
    public class DrawingHammerServer : SslServer
    {
        public DrawingHammerServer(X509Certificate2 certificate, int port) : base(certificate, port) { }

        public override BaseClientData HandleNewConnectedClient(TcpClient connectedClient, Stream stream)
        {
            //Um eigene Werte hinzuzufügen (Score) wir die Standard Methode überschrieben
            return new DrawingHammerClient(this, connectedClient, stream);
        }
    }
}
