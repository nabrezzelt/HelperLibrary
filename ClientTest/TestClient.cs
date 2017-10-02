using HelperLibrary.Networking;
using HelperLibrary.Networking.ClientServer;
using HelperLibrary.Networking.ClientServer.Packets;
using System;
using PacketLibrary;

namespace ClientDemo
{
    class TestClient
    {
        private static Client _client;
        private static string _uid;

        private static void Main(string[] args)
        {
            //Uid setzen:
            Console.Write("Set your Uid: ");
            _uid = Console.ReadLine();

            _client = new Client();

            _client.ConnectionLost += OnConnectionLost;
            _client.ConnectionSucceed += OnConnectionSucceed;
            _client.PacketReceived += OnPacketReceived;

            _client.Connect(NetworkUtilities.GetThisIPv4Adress(), 9999);            
        }

        private static void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            var packet = e.Packet;
            Console.WriteLine("Packet recived of type: {0}", nameof(packet));

            //Switch over all diffrent PacketTypes and handle them:
            switch (packet)
            {
                case BasePacket p:
                    Console.WriteLine("Packet is BasePacket");
                    //OnBasePacketReceived(p)
                    break;
                
                default:
                    Console.WriteLine("Unhandled Packet");
                    break;
            }
        }

        private static void OnConnectionSucceed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection successfull!");

            AuthenticateToServer();

            Console.WriteLine("Authetication-Packet send");
        }

        private static void OnConnectionLost(object sender, EventArgs e)
        {
            Console.WriteLine("Connection lost!");
        }

        private static void AuthenticateToServer()
        {
            _client.SendPacketToServer(new AuthenticationPacket(_uid, Router.ServerWildcard));
        }
    }
}
