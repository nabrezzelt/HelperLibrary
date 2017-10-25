using HelperLibrary.Logging;
using HelperLibrary.Networking.ClientServer.Packets;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace HelperLibrary.Networking.ClientServer
{
    public class Client
    {
        /// <summary>
        /// Fired when connection to server succeed.
        /// </summary>
        public event EventHandler ConnectionSucceed;

        /// <summary>
        /// Fired when connection to server is lost.
        /// </summary>
        public event EventHandler ConnectionLost;  
        
        /// <summary>
        /// Fired when client receives a packet.
        /// </summary>
        public event EventHandler<PacketReceivedEventArgs> PacketReceived;

        protected IPAddress ServerIP;
        protected int Port;
        protected TcpClient TcpClient;
        protected Stream ClientStream;

        /// <summary>
        /// Initialize the connection to the server.
        /// </summary>
        /// <param name="serverIP">Server IP</param>
        /// <param name="port">Port to connect</param>
        public void Connect(IPAddress serverIP, int port)
        {
            ServerIP = serverIP;
            Port = port;

            ConnectToServer();
            StartReceivingData();
            ConnectionSucceed?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void Connect(string serverIP, int port)
        {
            Connect(IPAddress.Parse(serverIP), port);
        }

        protected virtual void ConnectToServer()
        {
            TcpClient = new TcpClient();

            while (!TcpClient.Connected)
            {
                try
                {
                    Log.Info("Trying to connect to server at " + ServerIP + " on port " + Port + "...");

                    TcpClient.Connect(new IPEndPoint(ServerIP, Port));
                    ClientStream = TcpClient.GetStream();

                    Log.Info("Connected");
                }
                catch (Exception e)
                {
                    Log.Error(e.Message + Environment.NewLine);                                        
                }                
            }
        }

        private void StartReceivingData()
        {
            Log.Info("Starting incomming data handler...");
            Thread receiveDataThread = new Thread(HandleIncommingData);
            receiveDataThread.Start();
            Log.Info("Incomming data handler started.");
        }

        private void HandleIncommingData()
        {            
            try
            {
                byte[] buffer; //Daten
                byte[] dataSize = new byte[4]; //Länge

                int readBytes = ClientStream.Read(dataSize, 0, 4);

                while (readBytes != 4)
                {
                    readBytes += ClientStream.Read(dataSize, readBytes, 4 - readBytes);
                }
                var contentLength = BitConverter.ToInt32(dataSize, 0);

                buffer = new byte[contentLength];
                readBytes = 0;
                while (readBytes != buffer.Length)
                {
                    readBytes += ClientStream.Read(buffer, readBytes, buffer.Length - readBytes);
                }

                //Daten sind im Buffer-Array gespeichert
                PacketReceived?.Invoke(this, new PacketReceivedEventArgs(BasePacket.Deserialize(buffer), TcpClient));
            }
            catch (IOException ex)
            {
                Log.Info(ex.Message);
                Log.Info("Server connection lost!");
                ConnectionLost?.Invoke(this, EventArgs.Empty);
            }
        }        

        /// <summary>
        /// Close the connection to the server.
        /// </summary>
        public void Disconnect()
        {
            if (!TcpClient.Connected)
                throw new InvalidOperationException("You're not connected!");

            ClientStream.Close();
            TcpClient.Close();
            Log.Info("Disconnected!");
        }

        /// <summary>
        /// Send a packet to the server.
        /// </summary>
        /// <param name="packet">Packet to send</param>
        public virtual void SendPacketToServer(BasePacket packet)
        {
            if(!TcpClient.Connected)
                throw new InvalidOperationException("You're not connected!");

            byte[] packetBytes = BasePacket.Serialize(packet);

            var length = packetBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);            

            ClientStream.Write(lengthBytes, 0, 4); //Senden der Länge/Größe des Textes
            ClientStream.Write(packetBytes, 0, packetBytes.Length); //Senden der eingentlichen Daten/des Textes   
        }        
    }
}
