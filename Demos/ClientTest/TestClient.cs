using HelperLibrary.Networking;
using HelperLibrary.Networking.ClientServer;
using HelperLibrary.Networking.ClientServer.Packages;
using System;
using HelperLibrary.Logging;
using PackageLibrary;

namespace ClientDemo
{
    class TestClient
    {
        private static SslClient _client;
        private static string _uid;

        private static void Main(string[] args)
        {
            //Uid setzen:
            Console.Write("Set your Uid: ");
            _uid = Console.ReadLine();


            _client = new SslClient(NetworkUtilities.GetThisIPv4Adress(), true);

            _client.ConnectionLost += OnConnectionLost;
            _client.ConnectionSucceed += OnConnectionSucceed;
            _client.PackageReceived += OnPackageReceived;

            _client.Connect(NetworkUtilities.GetThisIPv4Adress(), 9999);            
        }

        private static void OnPackageReceived(object sender, PackageReceivedEventArgs e)
        {
            var package = e.Package;
            Console.WriteLine("Package recived of type: {0}", nameof(package));

            //Switch over all diffrent PackageTypes and handle them:
            switch (package)
            {                
                case AuthenticationResultPackage p:
                    if (p.Result == AuthenticationResult.Ok)
                    {
                        Log.Info("Authentication succeed");
                    }
                    else
                    {
                        Log.Warn("Authentication failed");
                                
                    }
                    break;

                case BasePackage p:
                    Console.WriteLine("Package is BasePackage");                    
                    break;

                default:
                    Console.WriteLine("Unhandled Package");
                    break;
            }
        }

        private static void OnConnectionSucceed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection successfull!");

            AuthenticateToServer();

            Console.WriteLine("Authetication-Package send");
        }

        private static void OnConnectionLost(object sender, EventArgs e)
        {
            Console.WriteLine("Connection lost!");
        }

        private static void AuthenticateToServer()
        {
            _client.SendPackageToServer(new AuthenticationPackage(_uid, Router.ServerWildcard));
        }
    }
}
