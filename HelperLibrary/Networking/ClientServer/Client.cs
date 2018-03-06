using HelperLibrary.Logging;
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
        /// Fired when client receives a package.
        /// </summary>
        public event EventHandler<PackageReceivedEventArgs> PackageReceived;

        public bool IsConnected { get; set; }

        protected IPAddress ServerIp;
        protected int Port;
        protected TcpClient TcpClient;
        protected Stream ClientStream;

        /// <summary>
        /// Initialize the connection to the server.
        /// </summary>
        /// <param name="serverIp">Server IP</param>
        /// <param name="port">Port to connect</param>
        public void Connect(IPAddress serverIp, int port)
        {
            ServerIp = serverIp;
            Port = port;

            ConnectToServer();
            StartReceivingData();
            ConnectionSucceed?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc cref="Connect(IPAddress, int)"/>
        public void Connect(string serverIp, int port)
        {
            Connect(IPAddress.Parse(serverIp), port);
        }

        protected virtual void ConnectToServer()
        {
            TcpClient = new TcpClient();

            while (!TcpClient.Connected)
            {
                try
                {
                    Log.Info("Trying to connect to server at " + ServerIp + " on port " + Port + "...");

                    TcpClient.Connect(new IPEndPoint(ServerIp, Port));
                    ClientStream = TcpClient.GetStream();

                    IsConnected = true;
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
                while (true)
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
                    PackageReceived?.Invoke(this,
                        new PackageReceivedEventArgs(BasePackage.Deserialize(buffer), TcpClient));
                }                
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
        /// Send a package to the server.
        /// </summary>
        /// <param name="package">Package to send</param>
        public void SendPackageToServer(BasePackage package)
        {
            if(!TcpClient.Connected)
                throw new InvalidOperationException("You're not connected!");

            byte[] packageBytes = BasePackage.Serialize(package);

            var length = packageBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);            

            ClientStream.Write(lengthBytes, 0, 4); //Senden der Länge/Größe des Textes
            ClientStream.Write(packageBytes, 0, packageBytes.Length); //Senden der eingentlichen Daten/des Textes   
        }        
    }
}
