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
using YeelightAPI.Events;
using YeelightAPI.Models;

namespace YeelightAPI
{
    /// <summary>
    /// Finds devices through LAN
    /// </summary>
    public static class DeviceLocator
    {
        #region Private Fields

        private const string _ssdpMessageTemplate = "M-SEARCH * HTTP/1.1\r\nHOST: {0}:1982\r\nMAN: \"ssdp:discover\"\r\nST: wifi_bulb";
        private static readonly List<object> _allPropertyRealNames = PROPERTIES.ALL.GetRealNames();
        private static readonly char[] _colon = new char[] { ':' };
        private static string _yeelightlocationMatch = "Location: yeelight://";

        #endregion Private Fields

        /// <summary>
        /// Notification Received event
        /// </summary>
        public static event DeviceFoundEventHandler OnDeviceFound;

        /// <summary>
        /// Notification Received event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void DeviceFoundEventHandler(object sender, DeviceFoundEventArgs e);

        #region Public Methods

        /// <summary>
        /// Discover devices in a specific Network Interface
        /// </summary>
        /// <param name="preferedInterface"></param>
        /// <returns></returns>
        public static async Task<List<Device>> Discover(NetworkInterface preferedInterface)
        {
            List<Task<List<Device>>> tasks = CreateDiscoverTasks(preferedInterface);
            List<Device> devices = new List<Device>();

            if (tasks.Count != 0)
            {
                await Task.WhenAll(tasks);

                devices.AddRange(tasks.SelectMany(t => t.Result).GroupBy(d => d.Hostname).Select(g => g.First()));
            }

            return devices;
        }

        /// <summary>
        /// Discover devices in LAN
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Device>> Discover()
        {
            List<Task<List<Device>>> tasks = new List<Task<List<Device>>>();
            List<Device> devices = new List<Device>();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces().Where(n => n.OperationalStatus == OperationalStatus.Up))
            {
                tasks.AddRange(CreateDiscoverTasks(ni));
            }

            if (tasks.Count != 0)
            {
                await Task.WhenAll(tasks);

                devices.AddRange(tasks.SelectMany(t => t.Result).GroupBy(d => d.Hostname).Select(g => g.First()));
            }

            return devices;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Create Discovery tasks for a specific Network Interface
        /// </summary>
        /// <param name="netInterface"></param>
        /// <returns></returns>
        private static List<Task<List<Device>>> CreateDiscoverTasks(NetworkInterface netInterface)
        {
            var devices = new ConcurrentDictionary<string, Device>();
            List<Task<List<Device>>> tasks = new List<Task<List<Device>>>();

            try
            {
                var ipProperties = netInterface.GetIPProperties();
                GatewayIPAddressInformation addr = ipProperties.GatewayAddresses.FirstOrDefault();
                var interfaceIndex = ipProperties.GetIPv4Properties().Index;
                if (addr != null && !addr.Address.ToString().Equals("0.0.0.0"))
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (UnicastIPAddressInformation ip in netInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                for (int cpt = 0; cpt < 3; cpt++)
                                {
                                    foreach (var multicastEndpoint in netInterface.GetIPProperties().MulticastAddresses.Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork).ToList())
                                    {
                                        Task<List<Device>> t = Task.Factory.StartNew<List<Device>>(() =>
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
                                                new MulticastOption(multicastEndpoint.Address, interfaceIndex));
                                            ssdpSocket.SendTo(Encoding.ASCII.GetBytes(string.Format(_ssdpMessageTemplate, multicastEndpoint.Address.ToString())), SocketFlags.None, new IPEndPoint(multicastEndpoint.Address, 1982));

                                            DateTime start = DateTime.Now;
                                            while (DateTime.Now - start < TimeSpan.FromSeconds(1))
                                            {
                                                int available = ssdpSocket.Available;

                                                if (available > 0)
                                                {
                                                    byte[] buffer = new byte[available];
                                                    int i = ssdpSocket.Receive(buffer, SocketFlags.None);

                                                    if (i > 0)
                                                    {
                                                        string response = Encoding.UTF8.GetString(buffer.Take(i).ToArray());
                                                        Device device = GetDeviceInformationsFromSsdpMessage(response);

                                                        //add only if no device already matching
                                                        if (devices.TryAdd(device.Hostname, device))
                                                        {
                                                            OnDeviceFound?.Invoke(null, new DeviceFoundEventArgs(device));
                                                        }
                                                    }
                                                }
                                                Thread.Sleep(10);
                                            }
                                            return devices.Values.ToList();
                                        });
                                        tasks.Add(t);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return tasks;
        }

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
                List<METHODS> supportedMethods = new List<METHODS>();
                string id = null, firmwareVersion = null;
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
                            string propertyValue = property[1].Trim();

                            if (_allPropertyRealNames.Contains(propertyName))
                            {
                                properties.Add(propertyName, propertyValue);
                            }
                            else if (propertyName == "id")
                            {
                                id = propertyValue;
                            }
                            else if (propertyName == "model")
                            {
                                if (!RealNameAttributeExtension.TryParseByRealName(propertyValue, out model))
                                {
                                    model = default(MODEL);
                                }
                            }
                            else if (propertyName == "support")
                            {
                                string[] supportedOperations = propertyValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                                foreach (string operation in supportedOperations)
                                {
                                    if (RealNameAttributeExtension.TryParseByRealName(operation, out METHODS method))
                                    {
                                        supportedMethods.Add(method);
                                    }
                                }
                            }
                            else if (propertyName == "fw_ver")
                            {
                                firmwareVersion = propertyValue;
                            }
                        }
                    }
                }
                return new Device(host, port, id, model, firmwareVersion, properties, supportedMethods);
            }

            return null;
        }

        #endregion Private Methods
    }
}