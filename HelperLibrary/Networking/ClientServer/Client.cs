using HelperLibrary.Logging;
using HelperLibrary.Networking.ClientServer.Exceptions;
using HelperLibrary.Networking.ClientServer.Packets;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace HelperLibrary.Networking.ClientServer
{
    public class Client : IClient
    {
        public event EventHandler ConnectionSucceed;
        public event EventHandler ConnectionLost;        
        public event EventHandler<PacketReceivedEventArgs> PacketReceived;

        private IPAddress _serverIP;
        private int _port;
        private TcpClient _tcpClient;
        private bool _debug;
        private NetworkStream _clientStream;

        public Client(bool debug = false)
        {
            _debug = debug;
        }

        public void Connect(IPAddress serverIP, int port)
        {
            _serverIP = serverIP;
            _port = port;

            ConnectToServer();
            StartReceivingData();
            ConnectionSucceed?.Invoke(this, EventArgs.Empty);
        }

        private void ConnectToServer()
        {
            _tcpClient = new TcpClient();

            while (!_tcpClient.Connected)
            {
                try
                {
                    Log.Info("Trying to connect to server at " + _serverIP + " on port " + _port + "...");

                    _tcpClient.Connect(new IPEndPoint(_serverIP, _port));

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
            _clientStream = _tcpClient.GetStream();

            try
            {
                byte[] buffer; //Daten
                byte[] dataSize = new byte[4]; //Länge

                int readBytes = _clientStream.Read(dataSize, 0, 4);

                while (readBytes != 4)
                {
                    readBytes += _clientStream.Read(dataSize, readBytes, 4 - readBytes);
                }
                var contentLength = BitConverter.ToInt32(dataSize, 0);

                buffer = new byte[contentLength];
                readBytes = 0;
                while (readBytes != buffer.Length)
                {
                    readBytes += _clientStream.Read(buffer, readBytes, buffer.Length - readBytes);
                }

                //Daten sind im Buffer-Array gespeichert
                PacketReceived?.Invoke(this, new PacketReceivedEventArgs(BasePacket.Deserialize(buffer), _tcpClient));
            }
            catch (IOException ex)
            {
                Log.Info(ex.Message);
                Log.Info("Server disconnected!");
                ConnectionLost?.Invoke(this, EventArgs.Empty);
            }
        }


        public void Connect(string serverIP, int port)
        {
            Connect(IPAddress.Parse(serverIP), port);
        }

        public void Disconnect()
        {
            if (!_tcpClient.Connected)
                throw new NotImplementedException();

            _tcpClient.Close();
            Log.Info("Disconnected!");
        }

        public void SendPacketToServer(BasePacket packet)
        {
            if(!_tcpClient.Connected)
                throw new NotImplementedException();

            byte[] packetBytes = BasePacket.Serialize(packet);

            var length = packetBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);

            if (_clientStream == null)
                _clientStream = _tcpClient.GetStream();

            _clientStream.Write(lengthBytes, 0, 4); //Senden der Länge/Größe des Textes
            _clientStream.Write(packetBytes, 0, packetBytes.Length); //Senden der eingentlichen Daten/des Textes   
        }        
    }
}
