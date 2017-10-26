using System.IO;
using System.Net.Sockets;
using HelperLibrary.Networking.ClientServer;

namespace ServerDemo
{
    public class DrawingHammerClient : BaseClientData
    {
        public int Score;

        public DrawingHammerClient(int score, Server serverInstance, TcpClient client, Stream stream) : base(serverInstance, client, stream)
        {
            Score = score;
        }

        public DrawingHammerClient(Server serverInstance, TcpClient client, Stream stream) : base(serverInstance, client, stream)
        {
            Score = 0;
        }
    }
}