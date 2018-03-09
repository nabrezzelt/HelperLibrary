using HelperLibrary.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using HelperLibrary.Networking.ClientServer.Packages;

namespace HelperLibrary.Networking.ClientServer
{
    public class BaseClientData
    {
        public string Uid { get; set; }

        public bool Authenticated { get; set; }

        public TcpClient TcpClient { get; }

        public Stream ClientStream { get; }

        private readonly Thread _clientThread;                
        private readonly Server _serverInstance;

        private readonly ConcurrentQueue<byte[]> _pendingDataToWrite = new ConcurrentQueue<byte[]>();
        private bool _sendingData;

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
        /// <param name="package">Package to send (must inherit <see cref="BasePackage"/>).</param>
        [Obsolete("Use EnqueueDataForWrite(object packet) instead!")]
        public void SendDataPackageToClient(BasePackage package)
        {
            EnqueueDataForWrite(package);
        }

        /// <summary>
        /// Enqueue the packet in the message queue to send this to the client.
        /// </summary>
        /// <param name="package">Package to send (must inherit <see cref="BasePackage"/>).</param>
        public void EnqueueDataForWrite(BasePackage package)
        {
            var packageBytes = BasePackage.Serialize(package);

            var length = packageBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);

            _pendingDataToWrite.Enqueue(lengthBytes);
            _pendingDataToWrite.Enqueue(packageBytes);

            lock (_pendingDataToWrite)
            {
                if (_sendingData)
                {
                    return;
                }

                _sendingData = true;
            }

            WriteData();
        }       

        private void WriteData()
        {
            byte[] buffer;

            try
            {
                if (_pendingDataToWrite.Count > 0 && _pendingDataToWrite.TryDequeue(out buffer))
                {
                    ClientStream.BeginWrite(buffer, 0, buffer.Length, WriteCallback, ClientStream);
                }
                else
                {
                    lock (_pendingDataToWrite)
                    {
                        _sendingData = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // maybe handle exception then
                Log.Debug(ex.ToString());
                lock (_pendingDataToWrite)
                {
                    _sendingData = false;
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
                // handle exception                
                Log.Debug(ex.ToString());
            }

            WriteData();
        }
    }
}
