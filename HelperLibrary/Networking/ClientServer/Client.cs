﻿using HelperLibrary.Logging;
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

        private TcpClient _tcpClient;
        private bool _debug;
        private NetworkStream _clientStream;

        public Client(bool debug = false)
        {
            _debug = debug;
        }

        public void Connect(IPAddress serverIP, int port)
        {
            ConnectToServer(serverIP, port);
            StartReceivingData();
        }

        private void ConnectToServer(IPAddress serverIP, int port)
        {
            _tcpClient = new TcpClient();

            while (!_tcpClient.Connected)
            {
                try
                {
                    Log.Info("Trying to connect to server at " + serverIP + " on port " + port + "...");

                    _tcpClient.Connect(new IPEndPoint(serverIP, port));
                    ConnectionSucceed?.Invoke(this, new EventArgs());

                    Log.Info("Connected");
                }
                catch (Exception e)
                {
                    Log.Fatal(e.Message);
                    throw;
                }
            }
        }

        private void StartReceivingData()
        {
            Thread receiveDataThread = new Thread(HandleIncommingData);
            receiveDataThread.Start();
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
                PacketReceived?.Invoke(this, new PacketReceivedEventArgs((BasePacket)BasePacket.Deserialize(buffer)));
            }
            catch (IOException ex)
            {
                Log.Info(ex.Message);
                Log.Info("Server disconnected!");
                Console.ReadLine();
                Environment.Exit(0);
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
            _clientStream.Write(lengthBytes, 0, 4); //Senden der Länge/Größe des Textes
            _clientStream.Write(packetBytes, 0, packetBytes.Length); //Senden der eingentlichen Daten/des Textes   
        }        
    }
}