using System;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using HelperLibrary.Networking;
using HelperLibrary.Networking.ClientServer;
using PackageLibrary;

namespace ServerDemo
{
    class TestApplicationServer
    {
        private static DrawingHammerServer _server;

        static void Main(string[] args)
        {
            var ip = NetworkUtilities.GetThisIPv4Adress();

            _server = new DrawingHammerServer(new X509Certificate2("certificate.pfx", "password"), ip, 9999);

            _server.ClientConnected += OnClientConnected;
            _server.PackageReceived += OnPackageReceived;
            _server.ClientDisconnected += OnClientDisconnected;

            _server.Start();                        
        }

        private static void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Console.WriteLine("New Client Connected");
            Console.WriteLine(e.Client.TcpClient.Client.RemoteEndPoint);
        }

        private static void OnPackageReceived(object sender, PackageReceivedEventArgs e)
        {
            var package = e.Package;
            Console.WriteLine("Package recived of type: {0}", nameof(package));

            //Switch over all different PackageTypes and handle them:
            switch (package)
            {
                case AuthenticationPackage p:
                    HandleAuthPackage(p, e.SenderTcpClient);
                    break;

                case BasePackage p:
                    Console.WriteLine("Package is BasePackage");                    
                    break;

                default:
                    Console.WriteLine("Unhandled Package");
                    break;
            }
        }

        private static void HandleAuthPackage(AuthenticationPackage authenticationPackage, TcpClient senderTcpClient)
        {
            Console.WriteLine("Clients count {0}", _server.Clients.Count);

            string clientUid = authenticationPackage.SenderUid;

            var connectedClient = _server.GetClientFromClientList(senderTcpClient);
            connectedClient.Uid = clientUid;
            Console.WriteLine("Client with {0} authenticated with UID: {1}", senderTcpClient.Client.RemoteEndPoint, clientUid);
        }

        private static void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine("Client disconncted with uid: " + e.ClientUid);
        }       
    }
}
