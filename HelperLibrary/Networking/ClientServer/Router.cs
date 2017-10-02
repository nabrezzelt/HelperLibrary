using HelperLibrary.Networking.ClientServer.Packets;
using System.Net.Sockets;

namespace HelperLibrary.Networking.ClientServer
{
    /// <summary>
    /// Router for packet distribution
    /// </summary>
    public class Router
    {
        public const string ServerWildcard = "server";
        public const string AllAuthenticatedWildCard = "all_auth";
        public const string AllNotAutheticatedWildCard = "all_no_auth";
        public const string AllWildCard = "all";

        private readonly Server _serverInstance;

        public Router(Server serverInstance)
        {
            _serverInstance = serverInstance;
        }       

        /// <summary>
        /// Distributes a Packet to a Wildcard or a specific UID.
        /// </summary>
        /// <param name="packet">Packet to distribute</param>
        /// <param name="senderTcpClient">TcpClient-Object of the sender</param>
        /// <param name="excludedClients">Array of clients, where the packet is not distributed</param>
        public void DistributePacket(BasePacket packet, TcpClient senderTcpClient, string[] excludedClients = null)
        {
            if (excludedClients == null)
                excludedClients = new string[] { };            

            switch (packet.DestinationUid)
            {
                case ServerWildcard:
                    _serverInstance.HandleIncommingData(packet, senderTcpClient);
                    break;

                case AllAuthenticatedWildCard:
                    foreach (BaseClientData client in _serverInstance.Clients)
                    {
                        if (client.Authenticated && !IsInArray(client.Uid, excludedClients))
                            client.SendDataPacketToClient(packet);
                    }
                    break;

                case AllNotAutheticatedWildCard:
                    foreach (BaseClientData client in _serverInstance.Clients)
                    {
                        if (!client.Authenticated && !IsInArray(client.Uid, excludedClients))
                            client.SendDataPacketToClient(packet);
                    }
                    break;

                case AllWildCard:
                    foreach (BaseClientData client in _serverInstance.Clients)
                    {
                        if (!IsInArray(client.Uid, excludedClients))
                            client.SendDataPacketToClient(packet);
                    }
                    break;
                default:
                    //Send to packet to DestinationUID
                    GetClientFromList(packet.DestinationUid)?.SendDataPacketToClient(packet);
                    break;
            }
        }


        /// <summary>
        /// Distributes a Packet to a Wildcard or a specific UID.
        /// </summary>
        /// <param name="packet">Packet to distribute</param>
        public void DistributePacket(BasePacket packet)
        {
            DistributePacket(packet, null);
        }

        /// <summary>
        /// Distributes a Packet to a Wildcard or a specific UID.
        /// </summary>
        /// <param name="packet">Packet to distribute</param>
        /// <param name="excludedClients">Array of clients, where the packet is not distributed</param>
        public void DistributePacket(BasePacket packet, string[] excludedClients)
        {
            DistributePacket(packet, null, excludedClients);
        }
        
        private static bool IsInArray(string uid, string[] uids)
        {
            foreach (string clientUid in uids)
            {
                if (uid == clientUid)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Findet den passenden Client welcher über diesen Socket mit dem Server verbunden ist.
        /// </summary>
        /// <param name="tcpClient">Socket mit dem der Client mit dem Server verbunden ist</param>
        /// <returns>Gefundenen Client andernfalls null.</returns>
        public BaseClientData GetClientFromList(TcpClient tcpClient)
        {
            foreach (BaseClientData client in _serverInstance.Clients)
            {
                if (client.TcpClient == tcpClient)
                {
                    return client;
                }
            }

            return null;
        }

        /// <summary>
        /// Findet den passenden Client welcher mit dieser UID mit dem Server verbunden ist.
        /// </summary>
        /// <param name="uid">Client UID</param>
        /// <returns>Gefundenen Client andernfalls null.</returns>
        public BaseClientData GetClientFromList(string uid)
        {
            foreach (BaseClientData client in _serverInstance.Clients)
            {
                if (client.Uid == uid)
                {
                    return client;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if a given ClientUID exists
        /// </summary>
        /// <param name="uid">ClientUID</param>
        /// <returns>true if Client exists, false if not</returns>
        public bool ClientUidExists(string uid)
        {
            foreach (BaseClientData client in _serverInstance.Clients)
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
