using System;
using System.Net.Sockets;
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
            _server = new DrawingHammerServer(9999);

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
                    Console.WriteLine("Packet is AuthenticationPacket");
                    OnAuthenticationPacketReceived(e.SenderTcpClient, p);
                    break;

                case BasePacket p:
                    Console.WriteLine("Packet is BasePacket");
                    //OnBasePacketReceived(p)
                    break;

                default:
                    Console.WriteLine("Unhandled Packet");
                    break;
            }
        }

        private static void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine("Client disconncted with uid: " + e.ClientUid);
        }

        private static void OnAuthenticationPacketReceived(TcpClient senderTcpClient, AuthenticationPacket packet)
        {
            //Zuordnen der UID des Connection-Objekts von dem das Paket kommt.
            DrawingHammerClient client = (DrawingHammerClient) _server.GetClientByUid(senderTcpClient);

            client.Uid = packet.SenderUid;
            client.Authenticated = true;

            Console.WriteLine("Client authenticated with UID: " + client.Uid);
        }
    }
}
