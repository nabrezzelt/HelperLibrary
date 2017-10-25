using System;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using HelperLibrary.Networking.ClientServer;
using HelperLibrary.Networking.ClientServer.Packets;
using PacketLibrary;

namespace ServerDemo
{
    class TestApplicationServer
    {
        private static DrawingHammerServer _server;

        static void Main(string[] args)
        {
            _server = new DrawingHammerServer(new X509Certificate2("certificate.pfx", "password", X509KeyStorageFlags.MachineKeySet), 9999);

            _server.ClientConnected += OnClientConnected;
            _server.PacketReceived += OnPacketReceived;
            _server.ClientDisconnected += OnClientDisconnected;

            _server.Start();                        
        }

        private static void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Console.WriteLine("New Client Connected");
            Console.WriteLine(e.Client.TcpClient.Client.RemoteEndPoint);
        }

        private static void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            var packet = e.Packet;
            Console.WriteLine("Packet recived of type: {0}", nameof(packet));

            //Switch over all different PacketTypes and handle them:
            switch (packet)
            {
                case AuthenticationPacket p:
                    HandleAuthPacket(p, e.SenderTcpClient);
                    break;

                case BasePacket p:
                    Console.WriteLine("Packet is BasePacket");                    
                    break;

                default:
                    Console.WriteLine("Unhandled Packet");
                    break;
            }
        }

        private static void HandleAuthPacket(AuthenticationPacket authenticationPacket, TcpClient senderTcpClient)
        {
            Console.WriteLine("Clients count {0}", _server.Clients.Count);

            string clientUid = authenticationPacket.SenderUid;

            var connectedClient = _server.GetClientByTcpClient(senderTcpClient);
            connectedClient.Uid = clientUid;
            Console.WriteLine("Client with {0} authenticated with UID: {1}", senderTcpClient.Client.RemoteEndPoint, clientUid);
        }

        private static void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine("Client disconncted with uid: " + e.ClientUid);
        }       
    }
}
