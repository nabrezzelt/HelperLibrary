using HelperLibrary.Logging;
using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using HelperLibrary.Networking.ClientServer.Packets;

namespace HelperLibrary.Networking.ClientServer
{
    public class SslClient : Client
    {
        private readonly string _serverName;
        private readonly bool _trustSelfGeneratedCertificated;

        public SslClient(string serverName, bool trustSelfGeneratedCertificated = false)
        {
            _serverName = serverName;
            _trustSelfGeneratedCertificated = trustSelfGeneratedCertificated;
        }

        protected override void ConnectToServer()
        {
            TcpClient = new TcpClient();

            while (!TcpClient.Connected)
            {
                try
                {
                    Log.Info("Trying to connect to server at " + ServerIP + " on port " + Port + "...");

                    TcpClient.Connect(new IPEndPoint(ServerIP, Port));
                    InitializeSslConnection(_serverName);
                    Log.SslStreamInformation((SslStream) ClientStream);
                    Log.Info("Connected");
                }
                catch (Exception e)
                {
                    Log.Error(e.Message + Environment.NewLine);
                }
            }
        }

        private void InitializeSslConnection(string serverName)
        {
            // Create an SSL stream that will close the client's stream.
            ClientStream = new SslStream(TcpClient.GetStream(), false, ValidateServerCertificate, null);

            var sslStream = (SslStream) ClientStream;
            // The server name must match the name on the server certificate.
            try
            {
                sslStream.AuthenticateAsClient(serverName, null, SslProtocols.Tls, false);
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                sslStream.Close();                              
            }            
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            String rootCAThumbprint = "11922B9F6E9E2D654B84C4C70D4120CCE6458292"; // write your code to get your CA's thumbprint

            // remove this line if commercial CAs are not allowed to issue certificate for your service.
            if ((sslPolicyErrors & (SslPolicyErrors.None)) > 0)
            {
                return true;
            }

            if ((sslPolicyErrors & (SslPolicyErrors.RemoteCertificateNameMismatch)) > 0 ||(sslPolicyErrors & (SslPolicyErrors.RemoteCertificateNotAvailable)) > 0)
            {
                return false; 
                
            }

            //// get last chain element that should contain root CA certificate
            //// but this may not be the case in partial chains
            //X509Certificate2 projectedRootCert = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
            //if (projectedRootCert.Thumbprint != rootCAThumbprint)
            //{
            //    return false;
            //}
            // execute certificate chaining engine and ignore only "UntrustedRoot" error
            X509Chain customChain = new X509Chain
            {
                ChainPolicy = {
                    VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority
                }
            };
            Boolean retValue = customChain.Build(chain.ChainElements[0].Certificate);
            // RELEASE unmanaged resources behind X509Chain class.
            customChain.Reset();
            return retValue;


            //if (sslPolicyErrors == SslPolicyErrors.None)
            //{                
            //    return true;
            //}          

            //////If certificate is selfgenerated trust it (and hope its encrypted :D)
            ////if (_trustSelfGeneratedCertificated && sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
            ////{
            ////    return true;
            ////}

            //Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            //// Do not allow this client to communicate with unauthenticated servers.
            //return false;
        }

        public override void SendPacketToServer(BasePacket packet)
        {
            //ToDo: Überprüfen warum er hier einen NetworkStream (wird auch in der ganzen Klasse verwendet?!?! K.a. wieso...) verwendet und kein SslStream
            //-> könnte problem mit der Schließenden Connection lösen vom Server.
            var stream = (SslStream) ClientStream;

            if (!TcpClient.Connected)
                throw new InvalidOperationException("You're not connected!");

            byte[] packetBytes = BasePacket.Serialize(packet);

            var length = packetBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);

            stream.Write(lengthBytes, 0, 4); //Senden der Länge/Größe des Textes
            stream.Write(packetBytes, 0, packetBytes.Length); //Senden der eingentlichen Daten/des Textes               
        }
    }    
}
