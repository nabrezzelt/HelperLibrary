using HelperLibrary.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using HelperLibrary.Networking.ClientServer.Packages;

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

        /// <summary>
        /// Indicates if the client is connected to the server.
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// <see cref="IPAddress"/> which the <see cref="TcpClient"/> is listen on .
        /// </summary>
        protected IPAddress ServerIp;

        /// <summary>
        /// Port which the <see cref="TcpClient"/> is listen on .
        /// </summary>
        protected int Port;

        protected TcpClient TcpClient;                
        protected Stream ClientStream;

        protected readonly ConcurrentQueue<byte[]> PendingDataToWrite = new ConcurrentQueue<byte[]>();
        protected bool SendingData;

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

        /// <summary>
        /// Opens the connection of the TcpClient.
        /// </summary>
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

        /// <summary>
        /// Starts the thread to receive and handle incomming data
        /// </summary>
        private void StartReceivingData()
        {
            Log.Info("Starting incomming data handler...");
            Thread receiveDataThread = new Thread(HandleIncommingData);
            receiveDataThread.Start();
            Log.Info("Incomming data handler started.");
        }

        /// <summary>
        /// Reads the incomming data from the <see cref="NetworkStream"/>.
        /// </summary>
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
        [Obsolete("Use EnqueueDataForWrite(BasePackage package) instead!")]
        public void SendPackageToServer(BasePackage package)
        {
            EnqueueDataForWrite(package);
        }
        /// <summary>
        /// Enqueue the packet in the message queue to send this to the server.
        /// </summary>
        /// <param name="package">Package to send (must inherit <see cref="BasePackage"/>).</param>
        public void EnqueueDataForWrite(BasePackage package)
        {
            if (!TcpClient.Connected)
                throw new InvalidOperationException("You're not connected!");

            var packageBytes = BasePackage.Serialize(package);

            var length = packageBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);

            PendingDataToWrite.Enqueue(lengthBytes);
            PendingDataToWrite.Enqueue(packageBytes);

            lock (PendingDataToWrite)
            {
                if (SendingData)
                {
                    return;
                }

                SendingData = true;
            }

            WriteData();
        }

        private void WriteData()
        {
            byte[] buffer;

            try
            {
                if (PendingDataToWrite.Count > 0 && PendingDataToWrite.TryDequeue(out buffer))
                {
                    ClientStream.BeginWrite(buffer, 0, buffer.Length, WriteCallback, ClientStream);
                }
                else
                {
                    lock (PendingDataToWrite)
                    {
                        SendingData = false;
                    }
                }
            }
            catch (Exception ex)
            {                
                Log.Debug(ex.ToString());
                lock (PendingDataToWrite)
                {
                    SendingData = false;
                }
            }
        }

        private void WriteCallback(IAsyncResult ar)
        {
            try
            {
                ClientStream.EndWrite(ar);
            }
            catch (Exception ex)
            {             
                Log.Debug(ex.ToString());
            }

            WriteData();
        }
    }
}
