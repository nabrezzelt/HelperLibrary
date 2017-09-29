using HelperLibrary.Networking.ClientServer;
using HelperLibrary.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperLibrary.Networking.ClientServer.Packets;

namespace ClientDemo
{
    class TestClient
    {
        private static Client _client = new Client();


        static void Main(string[] args)
        {
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
                    break;
            }
        }

        private static void OnConnectionSucceed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection successfull!");
        }

        private static void OnConnectionLost(object sender, EventArgs e)
        {
            Console.WriteLine("Connection lost!");
        }
    }
}
