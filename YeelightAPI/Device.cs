using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightAPI
{
    /// <summary>
    /// Yeelight Device
    /// </summary>
    public class Device : DeviceManager
    {
        private const int _defaultPort = 55443;
        private static string _yeelightlocationMatch = "Location: yeelight://";

        /// <summary>
        /// HostName
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Port number
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Constructor with a hostname and (optionally) a port number
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        public Device(string hostname, int port = _defaultPort)
        {
            this.Hostname = hostname;
            this.Port = port;
        }

        /// <summary>
        /// Constructor from a SSDP discovery response
        /// </summary>
        /// <param name="ssdpMessage"></param>
        public Device(string ssdpMessage)
        {
            if (ssdpMessage != null)
            {
                string[] split = ssdpMessage.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string part in split)
                {
                    if (part.StartsWith(_yeelightlocationMatch))
                    {
                        string url = part.Substring(_yeelightlocationMatch.Length);
                        string[] hostnameParts = url.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (hostnameParts.Length >= 1)
                        {
                            this.Hostname = hostnameParts[0];
                        }
                        if (hostnameParts.Length == 2)
                        {
                            int port = _defaultPort;
                            if (int.TryParse(hostnameParts[1], out port))
                            {
                                this.Port = port;
                            }
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Implicit connection with the device Hostname and port number
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            return this.Connect(this.Hostname, this.Port);
        }
    }
}
