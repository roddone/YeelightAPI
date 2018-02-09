using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YeelightAPI
{
    /// <summary>
    /// Finds devices through LAN
    /// </summary>
    public static class DeviceLocator
    {
        private static string _yeelightlocationMatch = "Location: yeelight://";
        private static readonly IPEndPoint _multicastEndPoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1982);
        private const string _ssdpMessage = "M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1982\r\nMAN: \"ssdp:discover\"\r\nST: wifi_bulb";
        private static readonly byte[] _ssdpDiagram = Encoding.ASCII.GetBytes(_ssdpMessage);

        /// <summary>
        /// Discover devices in LAN
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Device>> Discover()
        {
            List<Task> tasks = new List<Task>();
            List<Device> devices = new List<Device>();

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
                                            if (ssdpSocket.Available > 0)
                                            {
                                                byte[] buffer = new byte[ssdpSocket.Available];
                                                var i = ssdpSocket.Receive(buffer, SocketFlags.None);
                                                if (i > 0)
                                                {
                                                    string response = Encoding.UTF8.GetString(buffer.Take(i).ToArray());
                                                    Tuple<string, int> host = GetDeviceInformationsFromSsdpMessage(response);

                                                    if (!devices.Any(d => d.Hostname == host.Item1))
                                                    {
                                                        Device device = new Device(host.Item1, host.Item2);

                                                        //add only if no device already matching
                                                        if (!devices.Any(d => d.Hostname == device.Hostname))
                                                        {
                                                            devices.Add(device);
                                                        }
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
                await Task.WhenAll(tasks.ToArray());
            }

            return devices;
        }

        private static Tuple<string, int> GetDeviceInformationsFromSsdpMessage(string ssdpMessage)
        {
            if (ssdpMessage != null)
            {
                string[] split = ssdpMessage.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                string host = null;
                int port = Common.DefaultPort;

                foreach (string part in split)
                {
                    if (part.StartsWith(_yeelightlocationMatch))
                    {
                        string url = part.Substring(_yeelightlocationMatch.Length);
                        string[] hostnameParts = url.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (hostnameParts.Length >= 1)
                        {
                            host = hostnameParts[0];
                        }
                        if (hostnameParts.Length == 2)
                        {
                            int.TryParse(hostnameParts[1], out port);
                        }
                        break;
                    }
                }
                return new Tuple<string, int>(host, port);
            }

            return null;
        }
    }
}
