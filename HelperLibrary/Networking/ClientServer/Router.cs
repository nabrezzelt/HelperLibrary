using System.Net.Sockets;
using HelperLibrary.Networking.ClientServer.Packages;

namespace HelperLibrary.Networking.ClientServer
{
    /// <summary>
    /// Router for package handling and distribution.
    /// </summary>
    public class Router
    {
        public const string ServerWildcard = "server";
        public const string AllAuthenticatedWildCard = "all_auth";
        public const string AllNotAutheticatedWildCard = "all_no_auth";
        public const string AllWildCard = "all";

        private readonly Server _serverInstance;

        /// <summary>
        /// Initializes a new Router for package handling and distribution.
        /// </summary>
        /// <param name="serverInstance">Serverinstance to handle packages where are sent to the server.</param>
        public Router(Server serverInstance)
        {
            _serverInstance = serverInstance;
        }

        /// <summary>
        /// Distributes a Packet to a Wildcard or a specific UID.
        /// </summary>
        /// <param name="package">Packet to distribute</param>
        /// <param name="senderTcpClient">TcpClient-Object of the sender</param>
        /// <param name="excludedClients">Array of clients, where the packet is not distributed</param>
        public void DistributePackage(BasePackage package, TcpClient senderTcpClient, string[] excludedClients = null)
        {
            if (excludedClients == null)
                excludedClients = new string[] { };

            switch (package.DestinationUid)
            {
                case ServerWildcard:
                    _serverInstance.HandleIncommingData(package, senderTcpClient);
                    break;

                case AllAuthenticatedWildCard:
                    foreach (BaseClientData client in _serverInstance.Clients)
                    {
                        if (client.Authenticated && !IsInArray(client.Uid, excludedClients))
                            client.EnqueueDataForWrite(package);
                    }
                    break;

                case AllNotAutheticatedWildCard:
                    foreach (BaseClientData client in _serverInstance.Clients)
                    {
                        if (!client.Authenticated && !IsInArray(client.Uid, excludedClients))
                            client.EnqueueDataForWrite(package);
                    }
                    break;

                case AllWildCard:
                    foreach (BaseClientData client in _serverInstance.Clients)
                    {
                        if (!IsInArray(client.Uid, excludedClients))
                            client.EnqueueDataForWrite(package);
                    }
                    break;
                default:
                    //Send to package to DestinationUID
                    _serverInstance.GetClientFromClientList(package.DestinationUid)?.EnqueueDataForWrite(package);
                    break;
            }
        }


        /// <summary>
        /// Distributes a Package to a Wildcard or a specific UID.
        /// </summary>
        /// <param name="package">Package to distribute</param>
        public void DistributePackage(BasePackage package)
        {
            DistributePackage(package, null);
        }

        /// <summary>
        /// Distributes a Package to a Wildcard or a specific UID.
        /// </summary>
        /// <param name="package">Package to distribute</param>
        /// <param name="excludedClients">Array of clients, where the package is not distributed</param>
        public void DistributePackage(BasePackage package, string[] excludedClients)
        {
            DistributePackage(package, null, excludedClients);
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
    }
}
