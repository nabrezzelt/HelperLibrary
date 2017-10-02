using System;
using System.Net.Sockets;
using System.Threading;
using HelperLibrary.Networking.ClientServer.Packets;

namespace HelperLibrary.Networking.ClientServer
{
    public class BaseClientData
    {
        public string Uid;
        public bool Authenticated;
        public TcpClient TcpClient;
        private readonly Thread _clientThread;        
        private readonly NetworkStream _clientStream;
        private Server _serverInstance;

        public BaseClientData(Server serverInstance, TcpClient client)
        {
            Uid = "";

            _serverInstance = serverInstance;

            TcpClient = client;
            _clientStream = client.GetStream();

            //Starte für jeden Client nach dem Verbinden einen seperaten Thread in dem auf neue eingehende Nachrichten gehört/gewartet wird.
            _clientThread = new Thread(_serverInstance.DataIn);
            _clientThread.Start(client);
        }

        public void SendDataPacketToClient(object packet)
        {
            byte[] packetBytes = BasePacket.Serialize(packet);

            var length = packetBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);
            _clientStream.Write(lengthBytes, 0, 4); //Senden der Länge/Größe des Textes
            _clientStream.Write(packetBytes, 0, packetBytes.Length); //Senden der eingentlichen Daten/des Textes                                
        }
    }
}