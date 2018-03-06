using HelperLibrary.Logging;
using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace HelperLibrary.Networking.ClientServer
{
    public class SslClient : Client
    {
        private readonly string _serverName;
        private readonly bool _allowUntrustedRootCa;

        /// <summary>
        /// Initalize new SslClient.
        /// </summary>
        /// <param name="serverName">Servername specified in Servers certificate</param>
        /// <param name="allowUntrustedRootCa">Set if untrusted root CAs are allowed (for selfsigned Certificates it must be true).</param>
        public SslClient(string serverName, bool allowUntrustedRootCa = false)
        {
            _serverName = serverName;
            _allowUntrustedRootCa = allowUntrustedRootCa;
        }

        protected override void ConnectToServer()
        {
            TcpClient = new TcpClient();

            while (!TcpClient.Connected)
            {
                try
                {
                    Log.Info("Trying to connect to server at " + ServerIp + " on port " + Port + "...");

                    TcpClient.Connect(new IPEndPoint(ServerIp, Port));
                    InitializeSslConnection(_serverName);
                    Log.SslStreamInformation((SslStream) ClientStream);

                    IsConnected = true;
                    Log.Info("Connected");
                }
                catch (AuthenticationException)
                {
                    throw;
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
                sslStream.AuthenticateAsClient(serverName);
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

        /// <summary>
        /// Validates the certificate when <see cref="SslStream"/>.AuthenticateAsClient() is called.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns>True when the certificate is valid, False when the certificate is invalid.</returns>
        protected virtual bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (_allowUntrustedRootCa)
            {
                // remove this line if commercial CAs are not allowed to issue certificate for your service.
                if ((sslPolicyErrors & SslPolicyErrors.None) > 0)
                {
                    return true;
                }

                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) > 0 || (sslPolicyErrors & SslPolicyErrors.RemoteCertificateNotAvailable) > 0)
                {
                    return false;

                }

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
            }

            //Full Certificate-Validation
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }       
    }    
}
