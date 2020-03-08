using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
  ///   Finds devices through LAN
  /// </summary>
  public static class DeviceLocator
  {
    #region Private Fields

    private const string _ssdpMessage =
      "M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1982\r\nMAN: \"ssdp:discover\"\r\nST: wifi_bulb";

    private static readonly List<object> _allPropertyRealNames = PROPERTIES.ALL.GetRealNames();
    private static readonly char[] _colon = {':'};
    private static readonly IPEndPoint _multicastEndPoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1982);
    private static readonly byte[] _ssdpDiagram = Encoding.ASCII.GetBytes(DeviceLocator._ssdpMessage);
    private static readonly string _yeelightlocationMatch = "Location: yeelight://";

    #endregion Private Fields

    #region Depricated API. TODO: Remove

    /// <summary>
    ///   Notification Received event
    /// </summary>
    [Obsolete("Deprecated. Event will be removed in next release.")]
    public static event DeviceFoundEventHandler OnDeviceFound;

    /// <summary>
    ///   Notification Received event handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    [Obsolete("Deprecated. Event will be removed in next release.")]
    public delegate void DeviceFoundEventHandler(object sender, DeviceFoundEventArgs e);

    #region Public Methods

    /// <summary>
    ///   Discover devices in a specific Network Interface
    /// </summary>
    /// <param name="preferedInterface"></param>
    /// <returns></returns>
    [Obsolete("Deprecated. Use DiscoverAsync and overloads instead.")]
    public static async Task<List<Device>> Discover(NetworkInterface preferedInterface)
    {
      List<Task<List<Device>>> tasks = DeviceLocator.CreateDiscoverTasks(preferedInterface);

      List<Device>[] result = await Task.WhenAll(tasks);
      return result.SelectMany(devices => devices).ToList();
    }

    /// <summary>
    ///   Discover devices in LAN
    /// </summary>
    /// <returns></returns>
    [Obsolete("Deprecated. Use DiscoverAsync and overloads instead.")]
    public static async Task<List<Device>> Discover()
    {
      var tasks = new List<Task<List<Device>>>();

      foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces()
        .Where(n => n.OperationalStatus == OperationalStatus.Up))
      {
        tasks.AddRange(DeviceLocator.CreateDiscoverTasks(ni));
      }


      List<Device>[] result = await Task.WhenAll(tasks);
      return result.SelectMany(devices => devices).ToList();
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    ///   Create Discovery tasks for a specific Network Interface
    /// </summary>
    /// <param name="netInterface"></param>
    /// <returns></returns>
    [Obsolete("Deprecated. Use SearchNetworkForDevicesAsync instead")]
    private static List<Task<List<Device>>> CreateDiscoverTasks(NetworkInterface netInterface)
    {
      var devices = new ConcurrentDictionary<string, Device>();
      var tasks = new List<Task<List<Device>>>();

     
        GatewayIPAddressInformation addr = netInterface.GetIPProperties().GatewayAddresses.FirstOrDefault();

        if (addr != null && !addr.Address.ToString().Equals("0.0.0.0"))
        {
          if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
              netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
          {
            foreach (UnicastIPAddressInformation ip in netInterface.GetIPProperties().UnicastAddresses)
            {
              if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
              {
                for (var cpt = 0; cpt < 3; cpt++)
                {
                  Task<List<Device>> t = Task.Run(
                    async () =>
                    {
                      try
                      {
                        using (var ssdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
                        {
                          Blocking = false,
                          Ttl = 1,
                          UseOnlyOverlappedIO = true,
                          MulticastLoopback = false
                        })
                        {
                          ssdpSocket.Bind(new IPEndPoint(ip.Address, 0));
                          ssdpSocket.SetSocketOption(
                            SocketOptionLevel.IP,
                            SocketOptionName.AddMembership,
                            new MulticastOption(DeviceLocator._multicastEndPoint.Address));

                          ssdpSocket.SendTo(
                            DeviceLocator._ssdpDiagram,
                            SocketFlags.None,
                            DeviceLocator._multicastEndPoint);

                          var stopWatch = new Stopwatch();
                          stopWatch.Start();
                          while (stopWatch.Elapsed < TimeSpan.FromSeconds(1))
                          {
                            try
                            {
                              int available = ssdpSocket.Available;

                              if (available > 0)
                              {
                                var buffer = new byte[available];
                                int i = ssdpSocket.Receive(buffer, SocketFlags.None);

                                if (i > 0)
                                {
                                  string response = Encoding.UTF8.GetString(buffer.Take(i).ToArray());
                                  Device device = DeviceLocator.GetDeviceInformationsFromSsdpMessage(response);

                                  //add only if no device already matching
                                  if (devices.TryAdd(device.Hostname, device))
                                  {
                                    DeviceLocator.OnDeviceFound?.Invoke(null, new DeviceFoundEventArgs(device));
                                  }
                                }
                              }
                            }
                            catch (SocketException)
                            {
                            }

                            await Task.Delay(TimeSpan.FromMilliseconds(10));
                          }
                        }
                      }
                      catch (SocketException)
                      {
                      }

                      return devices.Values.ToList();
                    });

                  tasks.Add(t);
                }
              }
            }
          }
        }

      return tasks;
    }

    #endregion Depricated API. TODO: Remove


    #region Public Methods

    /// <summary>
    ///   Discover devices in LAN
    /// </summary>
    /// <returns></returns>
    public static async Task<IEnumerable<Device>> DiscoverAsync() =>
      await DeviceLocator.DiscoverAsync(deviceFoundReporter: null);

    /// <summary>
    ///   Discover devices in LAN
    /// </summary>
    /// <returns></returns>
    public static async Task<IEnumerable<Device>> DiscoverAsync(IProgress<Device> deviceFoundReporter)
    {
      var tasks = new List<Task<IEnumerable<Device>>>();

      foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces()
        .Where(n => n.OperationalStatus == OperationalStatus.Up))
      {
        tasks.Add(DeviceLocator.DiscoverAsync(ni, deviceFoundReporter));
      }


      IEnumerable<Device>[] result = await Task.WhenAll(tasks);
      return result.SelectMany(devices => devices).ToList();
    }

    /// <summary>
    ///   Discover devices in a specific Network Interface
    /// </summary>
    /// <param name="networkInterface"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Device>> DiscoverAsync(NetworkInterface networkInterface) =>
      await DeviceLocator.DiscoverAsync(networkInterface, null);

    /// <summary>
    ///   Discover devices in a specific Network Interface
    /// </summary>
    /// <param name="networkInterface"></param>
    /// <param name="deviceFoundReporter"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Device>> DiscoverAsync(
      NetworkInterface networkInterface,
      IProgress<Device> deviceFoundReporter) =>
      await DeviceLocator.SearchNetworkForDevicesAsync(networkInterface, deviceFoundReporter);

    #endregion Public Methods

    /// <summary>
    ///   Create Discovery tasks for a specific Network Interface
    /// </summary>
    /// <param name="netInterface"></param>
    /// <param name="deviceFoundCallback"></param>
    /// <returns></returns>
    private static async Task<IEnumerable<Device>> SearchNetworkForDevicesAsync(NetworkInterface netInterface, IProgress<Device> deviceFoundCallback)
    {
      GatewayIPAddressInformation addressInformation = netInterface.GetIPProperties().GatewayAddresses.FirstOrDefault();

      if (addressInformation == null || addressInformation.Address.ToString().Equals("0.0.0.0"))
      {
        return new List<Device>();
      }

      if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
          netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
      {
        return await Task.Run(
          () => netInterface.GetIPProperties().UnicastAddresses
            .Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork)
            .AsParallel()
            .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            .SelectMany(ip => DeviceLocator.CheckSocketForDevices(ip, deviceFoundCallback))
            .ToList());
      }

      return new List<Device>();
    }

    private static List<Device> CheckSocketForDevices(UnicastIPAddressInformation ip, IProgress<Device> deviceFoundCallback)
    {
      // Use hash table for faster lookup, than List.Contains
      var devices = new Dictionary<string, Device>();

      try // Catch socket creation exception
      {
        using (var ssdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
          Blocking = false,
          Ttl = 1,
          UseOnlyOverlappedIO = true,
          MulticastLoopback = false
        })
        {
          ssdpSocket.Bind(new IPEndPoint(ip.Address, 0));
          ssdpSocket.SetSocketOption(
            SocketOptionLevel.IP,
            SocketOptionName.AddMembership,
            new MulticastOption(DeviceLocator._multicastEndPoint.Address));

          ssdpSocket.SendTo(DeviceLocator._ssdpDiagram, SocketFlags.None, DeviceLocator._multicastEndPoint);

          var stopWatch = new Stopwatch();
          stopWatch.Start();
          while (stopWatch.Elapsed < TimeSpan.FromSeconds(1))
          {
            try // Catch socket read exception
            {
              int available = ssdpSocket.Available;

              if (available > 0)
              {
                var buffer = new byte[available];
                int numberOfBytesRead = ssdpSocket.Receive(buffer, SocketFlags.None);

                if (numberOfBytesRead > 0)
                {
                  string response = Encoding.UTF8.GetString(buffer.Take(numberOfBytesRead).ToArray());
                  Device device = DeviceLocator.GetDeviceInformationsFromSsdpMessage(response);

                  if (!devices.ContainsKey(device.Hostname))
                  {
                    devices.Add(device.Hostname, device);
                    deviceFoundCallback?.Report(device);
                  }
                }
              }
            }
            catch (SocketException)
            {
              // Ignore and continue
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(10));
          }
        }
      }
      catch (SocketException)
      {
        return new List<Device>();
      }

      return devices.Values.ToList();
    }

    /// <summary>
    ///   Gets the informations from a raw SSDP message (host, port)
    /// </summary>
    /// <param name="ssdpMessage"></param>
    /// <returns></returns>
    private static Device GetDeviceInformationsFromSsdpMessage(string ssdpMessage)
    {
      if (ssdpMessage != null)
      {
        string[] split = ssdpMessage.Split(new[] {Constants.LineSeparator}, StringSplitOptions.RemoveEmptyEntries);
        string host = null;
        int port = Constants.DefaultPort;
        var properties = new Dictionary<string, object>();
        var supportedMethods = new List<METHODS>();
        string id = null, firmwareVersion = null;
        MODEL model = default;

        foreach (string part in split)
        {
          if (part.StartsWith(DeviceLocator._yeelightlocationMatch))
          {
            string url = part.Substring(DeviceLocator._yeelightlocationMatch.Length);
            string[] hostnameParts = url.Split(DeviceLocator._colon, StringSplitOptions.RemoveEmptyEntries);
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
            string[] property = part.Split(DeviceLocator._colon);
            if (property.Length == 2)
            {
              string propertyName = property[0].Trim();
              string propertyValue = property[1].Trim();

              if (DeviceLocator._allPropertyRealNames.Contains(propertyName))
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
                  model = default;
                }
              }
              else if (propertyName == "support")
              {
                string[] supportedOperations = propertyValue.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

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