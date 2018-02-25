using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YeelightAPI.Core;
using YeelightAPI.Models;

namespace YeelightAPI
{
    /// <summary>
    /// Finds devices through LAN
    /// </summary>
    public static class DeviceLocator
    {
        #region Private Fields

        private const string _ssdpMessage = "M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1982\r\nMAN: \"ssdp:discover\"\r\nST: wifi_bulb";
        private static readonly List<object> _allPropertyRealNames = PROPERTIES.ALL.GetRealNames();
        private static readonly char[] _colon = new char[] { ':' };
        private static readonly IPEndPoint _multicastEndPoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1982);
        private static readonly byte[] _ssdpDiagram = Encoding.ASCII.GetBytes(_ssdpMessage);
        private static string _yeelightlocationMatch = "Location: yeelight://";

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Discover devices in LAN
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Device>> Discover()
        {
            List<Task> tasks = new List<Task>();
            ConcurrentBag<Device> devices = new ConcurrentBag<Device>();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                var addr = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (addr != null && !addr.Address.ToString().Equals("0.0.0.0"))
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                for (int cpt = 0; cpt < 3; cpt++)
                                {
                                    Task t = Task.Factory.StartNew(() =>
                                    {
                                        Socket ssdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
                                        {
                                            Blocking = false,
                                            Ttl = 1,
                                            UseOnlyOverlappedIO = true,
                                            MulticastLoopback = false,
                                        };
                                        ssdpSocket.Bind(new IPEndPoint(ip.Address, 0));
                                        ssdpSocket.SetSocketOption(
                                            SocketOptionLevel.IP,
                                            SocketOptionName.AddMembership,
                                            new MulticastOption(_multicastEndPoint.Address));

                                        ssdpSocket.SendTo(_ssdpDiagram, SocketFlags.None, _multicastEndPoint);

                                        DateTime start = DateTime.Now;
                                        while (DateTime.Now - start < TimeSpan.FromSeconds(1))
                                        {
                                            int available = ssdpSocket.Available;
                                            if (available > 0)
                                            {
                                                byte[] buffer = new byte[available];
                                                var i = ssdpSocket.Receive(buffer, SocketFlags.None);
                                                if (i > 0)
                                                {
                                                    string response = Encoding.UTF8.GetString(buffer.Take(i).ToArray());
                                                    Device device = GetDeviceInformationsFromSsdpMessage(response);

                                                    //add only if no device already matching
                                                    if (!devices.Any(d => d.Hostname == device.Hostname))
                                                    {
                                                        devices.Add(device);
                                                    }
                                                }
                                            }
                                            Thread.Sleep(10);
                                        }
                                    });
                                    tasks.Add(t);
                                }
                            }
                        }
                    }
                }
            }

            if (tasks.Count != 0)
            {
                await Task.WhenAll(tasks);
            }

            return devices.ToList();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the informations from a raw SSDP message (host, port)
        /// </summary>
        /// <param name="ssdpMessage"></param>
        /// <returns></returns>
        private static Device GetDeviceInformationsFromSsdpMessage(string ssdpMessage)
        {
            if (ssdpMessage != null)
            {
                string[] split = ssdpMessage.Split(new string[] { Constants.LineSeparator }, StringSplitOptions.RemoveEmptyEntries);
                string host = null;
                int port = Constants.DefaultPort;
                Dictionary<string, object> properties = new Dictionary<string, object>();
                string id = null;
                MODEL model = default(MODEL);

                foreach (string part in split)
                {
                    if (part.StartsWith(_yeelightlocationMatch))
                    {
                        string url = part.Substring(_yeelightlocationMatch.Length);
                        string[] hostnameParts = url.Split(_colon, StringSplitOptions.RemoveEmptyEntries);
                        if (hostnameParts.Length >= 1)
                        {
                            host = hostnameParts[0];
                        }
                        if (hostnameParts.Length == 2)
                        {
                            int.TryParse(hostnameParts[1], out port);
                        }
                    }
                    else
                    {
                        string[] property = part.Split(_colon);
                        if (property.Length == 2)
                        {
                            string propertyName = property[0].Trim();
                            if (_allPropertyRealNames.Contains(propertyName))
                            {
                                string propertyValue = property[1].Trim();
                                properties.Add(propertyName, propertyValue);
                            }
                            else if (propertyName == "id")
                            {
                                id = property[1].Trim();
                            }
                            else if (propertyName == "model")
                            {
                                if (!RealNameAttributeExtension.TryParseByRealName(property[1].Trim(), out model))
                                {
                                    model = default(MODEL);
                                }
                            }
                        }
                    }
                }
                return new Device(host, port, id, model, properties);
            }

            return null;
        }

        #endregion Private Methods
    }
}