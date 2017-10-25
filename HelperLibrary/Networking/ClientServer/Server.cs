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
        protected readonly Router Router;
        protected TcpListener Listener;
        protected readonly int Port;

        /// <summary>
        /// List containing all clients.
        /// </summary>
        public List<BaseClientData> Clients { get; } = new List<BaseClientData>();

        #region Events
        /// <summary>
        /// Fired when a new client is connected to the server.
        /// </summary>
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;

        /// <summary>
        /// Fired when a client closed/lost the connection to the server.
        /// </summary>
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;

        /// <summary>
        /// Fired when a packet is received by any client.
        /// </summary>
        public event EventHandler<PacketReceivedEventArgs> PacketReceived;

        protected void OnClientConnected(ClientConnectedEventArgs e)
        {
            ClientConnected?.Invoke(this, e);
        }

        protected void OnClientDisconnected(ClientDisconnectedEventArgs e)
        {
            ClientDisconnected?.Invoke(this, e);
        }

        protected void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            PacketReceived?.Invoke(sender, e);
        }
        #endregion

        /// <summary>
        /// Initalize a new Server on a given port.
        /// </summary>
        /// <param name="port"></param>
        protected Server(int port)
        {
            Port = port;
            Router = new Router(this);

            InitializeListener();
        }

        /// <summary>
        /// Starts the server on given IP and port.
        /// </summary>
        public void Start()
        {            
            Thread listenForNewClients = new Thread(ListenOnNewClients);
            listenForNewClients.Start();
        }

        protected void InitializeListener()
        {
            Log.Info("Starting server on " + NetworkUtilities.GetThisIPv4Adress() + " on port " + Port);

            Listener = new TcpListener(new IPEndPoint(IPAddress.Parse(NetworkUtilities.GetThisIPv4Adress()), Port));

            Log.Info("Server started. Waiting for new Client connections...");
        }

        protected virtual void ListenOnNewClients()
        {
            Listener.Start();

            while (true)
            {
                TcpClient connectedClient = Listener.AcceptTcpClient();

                var client = HandleNewConnectedClient(connectedClient, connectedClient.GetStream());

                Clients.Add(client);                
                OnClientConnected(new ClientConnectedEventArgs(client));
                Log.Info("New Client connected (IP: " + connectedClient.Client.RemoteEndPoint + ")");
            }                        
        }
        
        public abstract BaseClientData HandleNewConnectedClient(TcpClient connectedClient, Stream stream);

        internal void DataIn(object clientData)
        {
            BaseClientData client = (BaseClientData)clientData;
            var clientStream = client.ClientStream;
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
                    Router.DistributePacket((BasePacket)BasePacket.Deserialize(buffer), client.TcpClient);
                }
            }            
            catch (IOException e)
            {
                Log.Debug(e.ToString());
                BaseClientData disconnectedClient = Router.GetClientFromList(client.TcpClient);
                Log.Info("Client disconnected with UID: " + disconnectedClient.Uid);
                Clients.Remove(disconnectedClient);
                Log.Info("Client removed from list.");

                OnClientDisconnected(new ClientDisconnectedEventArgs(disconnectedClient.Uid));
            }
        }

        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="senderTcpClient"></param>
        public void HandleIncommingData(BasePacket packet, TcpClient senderTcpClient)
        {
            OnPacketReceived(senderTcpClient, new PacketReceivedEventArgs(packet, senderTcpClient));
        }

        /// <summary>
        /// Get a registred client by uid.
        /// </summary>
        /// <param name="clientUid">Uid of the client</param>
        /// <returns></returns>
        public BaseClientData GetClientByUid(string clientUid)
        {
            return Router.GetClientFromList(clientUid);
        }

        /// <summary>
        /// Get a connected client by connection.
        /// </summary>
        /// <param name="tcpClient"></param>
        /// <returns></returns>
        public BaseClientData GetClientByTcpClient(TcpClient tcpClient)
        {
            return Router.GetClientFromList(tcpClient);
        }
    }
}
