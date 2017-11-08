using HelperLibrary.Logging;
using HelperLibrary.Networking.ClientServer.Packages;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace HelperLibrary.Networking.ClientServer
{
    public class BaseClientData
    {
        public string Uid { get; set; }
        public bool Authenticated { get; set; }
        public TcpClient TcpClient { get; }
        public Stream ClientStream { get; }

        private readonly Thread _clientThread;                
        private Server _serverInstance;

        /// <summary>
        /// Creates a new Client, who handles reading and sending of packages.
        /// </summary>
        /// <param name="serverInstance">Serverinstance to handle incomming packages.</param>
        /// <param name="client"><see cref="TcpClient"/> of the</param>
        /// <param name="stream">Stream to read.</param>
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

        /// <summary>
        /// Sends a package to this client.
        /// </summary>
        /// <param name="package">Package to send (must inherit by <see cref="BasePackage"/>).</param>
        public void SendDataPackageToClient(object package)
        {
            byte[] packageBytes = BasePackage.Serialize(package);

            var length = packageBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);
            ClientStream.Write(lengthBytes, 0, 4); //Senden der Länge/Größe des Textes
            ClientStream.Write(packageBytes, 0, packageBytes.Length); //Senden der eingentlichen Daten/des Textes   

            Log.Debug("Packet send to: " + Uid);
        }
    }
}