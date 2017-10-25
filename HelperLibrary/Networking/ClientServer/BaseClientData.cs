using System;
using System.IO;
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
        public readonly Stream ClientStream;
        private readonly Thread _clientThread;                
        private Server _serverInstance;

        public BaseClientData(Server serverInstance, TcpClient client, Stream stream)
        {
            Uid = "";

            _serverInstance = serverInstance;

            TcpClient = client;
            ClientStream = stream;

            //Starte für jeden Client nach dem Verbinden einen seperaten Thread in dem auf neue eingehende Nachrichten gehört/gewartet wird.
            _clientThread = new Thread(_serverInstance.DataIn);
            _clientThread.Start(this);
        }

        public void SendDataPacketToClient(object packet)
        {
            byte[] packetBytes = BasePacket.Serialize(packet);

            var length = packetBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);
            ClientStream.Write(lengthBytes, 0, 4); //Senden der Länge/Größe des Textes
            ClientStream.Write(packetBytes, 0, packetBytes.Length); //Senden der eingentlichen Daten/des Textes                                
        }
    }
}