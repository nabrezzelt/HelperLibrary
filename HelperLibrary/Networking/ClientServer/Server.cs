using HelperLibrary.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using HelperLibrary.Networking.ClientServer.Packages;

namespace HelperLibrary.Networking.ClientServer
{
    public abstract class Server
    {
        /// <summary>
        /// Used for sending and disritution of packages.
        /// </summary>
        public Router Router { get; set; }
        protected TcpListener Listener { get; set; }
        protected IPAddress Ip { get; }
        protected int Port { get; }

        /// <summary>
        /// List of all connected clients.
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
        /// Fired when a package is received by any client.
        /// </summary>
        public event EventHandler<PackageReceivedEventArgs> PackageReceived;

        protected void OnClientConnected(ClientConnectedEventArgs e)
        {
            ClientConnected?.Invoke(this, e);
        }

        protected void OnClientDisconnected(ClientDisconnectedEventArgs e)
        {
            ClientDisconnected?.Invoke(this, e);
        }

        protected void OnPackageReceived(object sender, PackageReceivedEventArgs e)
        {
            PackageReceived?.Invoke(sender, e);
        }
        #endregion

        /// <summary>
        /// Initalizes a new Server on a given port and IP address.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        protected Server(IPAddress ip, int port)
        {
            Ip = ip;
            Port = port;
            Router = new Router(this);

            InitializeListener();
        }

        /// <inheritdoc cref="Server(IPAddress, int)"/> 
        /// <exception cref="InvalidOperationException">Exception is thrown when IP address could not be parsed.</exception>      
        protected Server(string ip, int port)
        {
            if (!IPAddress.TryParse(ip, out var validIp))
            {
                throw new InvalidOperationException($"IP address ({ip}) must be valid!");
            }

            Ip = validIp;
            Port = port;
            Router = new Router(this);

            InitializeListener();
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            Thread listenForNewClients = new Thread(ListenOnNewClients);
            listenForNewClients.Start();

            Log.Info("Server started.");
            Log.Info("Waiting for new client connections...");
        }

        protected void InitializeListener()
        {
            Log.Info("Configured server for " + Ip + " on port " + Port);

            Listener = new TcpListener(new IPEndPoint(Ip, Port));
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
                Log.Info("New client connected (IP: " + connectedClient.Client.RemoteEndPoint + ")");
            }                        
        }

        /// <summary>
        /// Creates a new object to hold and handle packages. Override this method to define additional Properties (e.g. a score or a user object).
        /// </summary>
        /// <param name="connectedClient"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public virtual BaseClientData HandleNewConnectedClient(TcpClient connectedClient, Stream stream)
        {            
            return new BaseClientData(this, connectedClient, stream);
        }

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
                    Router.DistributePackage(BasePackage.Deserialize<BasePackage>(buffer), client.TcpClient);
                }
            }            
            catch (IOException e)
            {
                Log.Debug(e.ToString());
                BaseClientData disconnectedClient = GetClientFromClientList(client.TcpClient);
                Log.Info("Client disconnected with Uid: " + disconnectedClient.Uid);
                Clients.Remove(disconnectedClient);
                Log.Info("Client removed from list.");

                OnClientDisconnected(new ClientDisconnectedEventArgs(disconnectedClient.Uid));
            }
        }

        /// <summary>    
        /// Handles packets who are sent to the server. 
        /// </summary>
        /// <param name="package"></param>
        /// <param name="senderTcpClient"></param>
        public void HandleIncommingData(BasePackage package, TcpClient senderTcpClient)
        {
            OnPackageReceived(senderTcpClient, new PackageReceivedEventArgs(package, senderTcpClient));
        }        

        /// <summary>
        /// Returns a connected client by his <see cref="TcpClient"/>.
        /// </summary>
        /// <param name="tcpClient"><see cref="TcpClient"/> of the client.</param>
        /// <returns>Returns the found client. If no client is found, null is returned.</returns>
        public BaseClientData GetClientFromClientList(TcpClient tcpClient)
        {
            foreach (BaseClientData client in Clients)
            {
                if (client.TcpClient == tcpClient)
                {
                    return client;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a connected client with the given uid.
        /// </summary>
        /// <param name="clientUid">Client Uid</param>
        /// <returns>Returns the found client. If no client is found, null is returned.</returns>
        public BaseClientData GetClientFromClientList(string clientUid)
        {
            foreach (BaseClientData client in Clients)
            {
                if (client.Uid == clientUid)
                {
                    return client;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if a client with this Uid exists.
        /// </summary>
        /// <param name="uid">Client Uid</param>
        /// <returns>True if Client exists, False if the client does not exists</returns>
        public bool ClientUidExists(string uid)
        {
            foreach (BaseClientData client in Clients)
            {
                if (client.Uid == uid)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
