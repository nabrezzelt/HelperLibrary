using System.Net;

namespace HelperLibrary.Networking
{
    public class NetworkUtilities
    {
        /// <summary>
        /// Returns the own IPv4 Address of this device. If no Address is set 127.0.0.1 will be returned.
        /// </summary>
        /// <returns>IPv4-Address as string</returns>
        public static string GetThisIPv4Adress()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }
    }
}
