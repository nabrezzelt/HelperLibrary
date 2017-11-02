using System;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace HelperLibrary.Logging
{
    public class Log
    {
        private const ConsoleColor ColorFatal = ConsoleColor.Red;
        private const ConsoleColor ColorError = ConsoleColor.DarkRed;
        private const ConsoleColor ColorWarn = ConsoleColor.DarkYellow;
        private const ConsoleColor ColorDebug = ConsoleColor.DarkCyan;

        public static void Debug(string message)
        {
            ClearCurrentConsoleLine();
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ColorDebug;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }

        public static void Info(string message, bool desktopClient = false)
        {
            ClearCurrentConsoleLine();
            Console.WriteLine(message);
        }

        public static void Warn(string message)
        {
            ClearCurrentConsoleLine();
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ColorWarn;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }

        public static void Error(string message)
        {
            ClearCurrentConsoleLine();
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ColorError;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }

        public static void Fatal(string message)
        {
            ClearCurrentConsoleLine();
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ColorFatal;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }

        public static void ClearCurrentConsoleLine()
        {
            try
            {
                var currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, currentLineCursor);
            }
            catch (IOException) { }
        }

        #region SslStream logging
        public static void SslStreamInformation(SslStream stream)
        {
            try
            {
                DisplaySecurityLevel(stream);
                DisplaySecurityServices(stream);
                DisplayStreamProperties(stream);
                DisplayCertificateInformation(stream);
            }
            catch (Exception e)
            {
                Fatal("Showing SslStream information failed");
                Fatal(e.ToString());                
            }
        }        

        public static void DisplaySecurityLevel(SslStream stream)
        {
            Console.WriteLine("Cipher: {0} strength {1}", stream.CipherAlgorithm, stream.CipherStrength);
            Console.WriteLine("Hash: {0} strength {1}", stream.HashAlgorithm, stream.HashStrength);
            Console.WriteLine("Key exchange: {0} strength {1}", stream.KeyExchangeAlgorithm, stream.KeyExchangeStrength);
            Console.WriteLine("Protocol: {0}", stream.SslProtocol);
        }

        public static void DisplaySecurityServices(SslStream stream)
        {
            Console.WriteLine("Is authenticated: {0} as server? {1}", stream.IsAuthenticated, stream.IsServer);
            Console.WriteLine("IsSigned: {0}", stream.IsSigned);
            Console.WriteLine("Is Encrypted: {0}", stream.IsEncrypted);
        }

        public static void DisplayStreamProperties(SslStream stream)
        {
            Console.WriteLine("Can read: {0}, write {1}", stream.CanRead, stream.CanWrite);
            Console.WriteLine("Can timeout: {0}", stream.CanTimeout);
        }

        public static void DisplayCertificateInformation(SslStream stream)
        {
            Console.WriteLine("Certificate revocation list checked: {0}", stream.CheckCertRevocationStatus);

            X509Certificate localCertificate = stream.LocalCertificate;
            if (stream.LocalCertificate != null)
            {
                Console.WriteLine("Local cert was issued to {0} and is valid from {1} until {2}.",
                    localCertificate.Subject,
                    localCertificate.GetEffectiveDateString(),
                    localCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Local certificate is null.");
            }

            // Display the properties of the client's certificate.
            X509Certificate remoteCertificate = stream.RemoteCertificate;
            if (remoteCertificate != null)
            {
                Console.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                    remoteCertificate.Subject, remoteCertificate.GetEffectiveDateString(),
                    remoteCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Remote certificate is null.");
            }
        }
        #endregion
    }
}
