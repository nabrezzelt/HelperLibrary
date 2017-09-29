using HelperLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HelperLibrary.Networking.ClientServer
{
    class Server
    {
        private TcpListener _listener;
        private int _port;
        private bool _debug;

        public Server(int port, bool debug = false)
        {
            _port = port;
            _debug = debug;
        }

        public void Start()
        {
            Log.Info("Starting server on " + NetworkUtilities.GetThisIPv4Adress());

            _listener = new TcpListener(new IPEndPoint(IPAddress.Parse(NetworkUtilities.GetThisIPv4Adress()), _port));

            Log.Info("Server started. Waiting for new Client connections...");
        }
    }
}
