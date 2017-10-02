using HelperLibrary.Logging;
using HelperLibrary.Networking.ClientServer.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace HelperLibrary.Networking.ClientServer
{
    public abstract class Server
    {
        private readonly Router _router;
        private TcpListener _listener;
        private readonly int _port;
        public List<BaseClientData> Clients { get; } = new List<BaseClientData>();

        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        public event EventHandler<PacketReceivedEventArgs> PacketReceived;


        protected Server(int port)
        {
            _port = port;
            _router = new Router(this);

            InitializeListener();
        }

        public void Start()
        {            
            Thread listenForNewClients = new Thread(ListenOnNewClients);
            listenForNewClients.Start();
        }

        protected void InitializeListener()
        {
            Log.Info("Starting server on " + NetworkUtilities.GetThisIPv4Adress() + " on port " + _port);

            _listener = new TcpListener(new IPEndPoint(IPAddress.Parse(NetworkUtilities.GetThisIPv4Adress()), _port));

            Log.Info("Server started. Waiting for new Client connections...");
        }

        protected void ListenOnNewClients()
        {
            _listener.Start();

            while (true)
            {
                TcpClient connectedClient = _listener.AcceptTcpClient();

                var client = HandleNewConnectedClient(connectedClient);

                Clients.Add(client);                
                ClientConnected?.Invoke(this, new ClientConnectedEventArgs(client));
                Log.Info("New Client connected (IP: " + connectedClient.Client.RemoteEndPoint + ")");
            }            
        }

        protected abstract BaseClientData HandleNewConnectedClient(TcpClient connectedClient);

        public void DataIn(object tcpClient)
        {
            TcpClient client = (TcpClient)tcpClient;
            NetworkStream clientStream = client.GetStream();
            try
            {
                while (true)
                {                    
                    byte[] buffer; //Daten
                    byte[] dataSize = new byte[4]; //Länge

                    int readBytes = clientStream.Read(dataSize, 0, 4);

                    while (readBytes != 4)
                    {
                        readBytes += clientStream.Read(dataSize, readBytes, 4 - readBytes);
                    }
                    var contentLength = BitConverter.ToInt32(dataSize, 0);

                    buffer = new byte[contentLength];
                    readBytes = 0;
                    while (readBytes != buffer.Length)
                    {
                        readBytes += clientStream.Read(buffer, readBytes, buffer.Length - readBytes);
                    }

                    //Daten sind im Buffer-Array
                    _router.DistributePacket((BasePacket)BasePacket.Deserialize(buffer), client);
                }
            }            
            catch (IOException)
            {
                BaseClientData disconnectedClient = _router.GetClientFromList(client);
                Log.Info("Client disconnected with UID: " + disconnectedClient.Uid);
                Clients.Remove(disconnectedClient);
                Log.Info("Client removed from list.");

                ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(disconnectedClient.Uid));
            }
        }

        public void HandleIncommingData(BasePacket packet, TcpClient senderTcpClient)
        {
            PacketReceived?.Invoke(senderTcpClient, new PacketReceivedEventArgs(packet, senderTcpClient));
        }

        public BaseClientData GetClientByUid(string clientUid)
        {
            return _router.GetClientFromList(clientUid);
        }

        public BaseClientData GetClientByUid(TcpClient tcpClient)
        {
            return _router.GetClientFromList(tcpClient);
        }
    }
}
