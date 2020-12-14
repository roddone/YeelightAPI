#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YeelightAPI.Core;
using YeelightAPI.Models;

#endregion

namespace YeelightAPI
{
    /// <summary>
    ///   Finds devices through LAN
    /// </summary>
    public static class DeviceLocator
    {
        #region Constructors

        static DeviceLocator() => DeviceLocator.MaxRetryCount = 1;

        #endregion

        #region Private Fields

        private const string _ssdpMessage =
          "M-SEARCH * HTTP/1.1\r\nHOST: {0}:1982\r\nMAN: \"ssdp:discover\"\r\nST: wifi_bulb";

        private const int RetryDelayInMilliseconds = 10;
        private static readonly List<object> _allPropertyRealNames = PROPERTIES.ALL.GetRealNames();
        private static readonly char[] _colon = { ':' };
        private static readonly string _defaultMulticastIPAddress = "239.255.255.250";
        private static readonly string _yeelightlocationMatch = "Location: yeelight://";

        #endregion Private Fields

        /// <summary>
        ///   Retry count for network sockets lookup to find devices.
        /// </summary>
        /// <value>The number of lookup retries. Default is 1.</value>
        /// <remarks>
        ///   A single iteration will take a maximum of 1 second. Each iteration will poll in intervals of 10 ms to listen
        ///   to an IP for devices. This means the value of <see cref="MaxRetryCount" /> is equivalent to execution time in
        ///   seconds.
        /// </remarks>
        public static int MaxRetryCount { get; set; }

        /// <summary>
        ///   Use all available multicast addresses for the discovery instead of the default multicast address only
        /// </summary>
        /// <remarks>Setting this parameter to true may result in a longer discovery time</remarks>
        public static bool UseAllAvailableMulticastAddresses { get; set; } = false;

        /// <summary>
        ///   Default multicast IP address used for the discovery. Can be changed to allow discovery on specific network
        ///   configurations
        /// </summary>
        public static string DefaultMulticastIPAddress { get; set; } = DeviceLocator._defaultMulticastIPAddress;

        #region Async API Methods

        /// <summary>
        ///   Discover devices in LAN
        /// </summary>
        /// <returns>A <see cref="Task" /> that contains a collection of all discovered <see cref="Device" /> instances.</returns>
        public static async Task<IEnumerable<Device>> DiscoverAsync() =>
          await DeviceLocator.DiscoverAsync(deviceFoundReporter: null);

        /// <summary>
        ///   Discover devices in LAN
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task" /> that contains a collection of all discovered <see cref="Device" /> instances.</returns>
        /// <exception cref="OperationCanceledException">Thrown when operation was cancelled by the caller.</exception>
        public static async Task<IEnumerable<Device>> DiscoverAsync(CancellationToken cancellationToken) =>
          await DeviceLocator.DiscoverAsync(deviceFoundReporter: null, cancellationToken);

        /// <summary>
        ///   Discover devices in LAN
        /// </summary>
        /// <param name="deviceFoundReporter">
        ///   An implementation of <see cref="IProgress{T}" /> to handle discovered devices as soon
        ///   they are available.
        /// </param>
        /// <returns>A <see cref="Task" /> that contains a collection of all discovered <see cref="Device" /> instances.</returns>
        public static async Task<IEnumerable<Device>> DiscoverAsync(IProgress<Device> deviceFoundReporter) =>
          await DeviceLocator.DiscoverAsync(deviceFoundReporter, CancellationToken.None);

        /// <summary>
        ///   Discover devices in LAN
        /// </summary>
        /// <param name="deviceFoundReporter">
        ///   An implementation of <see cref="IProgress{T}" /> to handle discovered devices as soon
        ///   they are available.
        /// </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task" /> that contains a collection of all discovered <see cref="Device" /> instances.</returns>
        /// <exception cref="OperationCanceledException">Thrown when operation was cancelled by the caller.</exception>
        public static async Task<IEnumerable<Device>> DiscoverAsync(
          IProgress<Device> deviceFoundReporter,
          CancellationToken cancellationToken)
        {
            IEnumerable<Task<DiscoveryResult>> tasks = NetworkInterface.GetAllNetworkInterfaces()
              .Where(networkInterface => networkInterface.OperationalStatus == OperationalStatus.Up)
              .Select(
                async networkInterface => await DeviceLocator.SearchNetworkForDevicesAsync(networkInterface, deviceFoundReporter, cancellationToken));

            DiscoveryResult[] results = await Task.WhenAll(tasks);

            //no results and exceptions has occured
            if (results.All(result => !result.HasResult && result.HasError))
            {
                throw new DeviceDiscoveryException("An error occurred during accessing a network socket. See the exception's property 'SocketExceptions' for more details.", results.SelectMany(result => result.Exceptions.SelectMany(e => e.SocketExceptions)));
            }

            return results
              .SelectMany(d => d.Devices)
              .GroupBy(d => d.Hostname)
              .Select(g => g.FirstOrDefault());
        }

        /// <summary>
        ///   Discover devices in a specific Network Interface
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <returns>A <see cref="Task" /> that contains a collection of all discovered <see cref="Device" /> instances.</returns>
        public static async Task<IEnumerable<Device>> DiscoverAsync(NetworkInterface networkInterface) =>
          await DeviceLocator.DiscoverAsync(networkInterface, CancellationToken.None);

        /// <summary>
        ///   Discover devices in a specific Network Interface
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task" /> that contains a collection of all discovered <see cref="Device" /> instances.</returns>
        /// <exception cref="OperationCanceledException">Thrown when operation was cancelled by the caller.</exception>
        public static async Task<IEnumerable<Device>> DiscoverAsync(
          NetworkInterface networkInterface,
          CancellationToken cancellationToken) =>
          await DeviceLocator.DiscoverAsync(networkInterface, null, cancellationToken);

        /// <summary>
        ///   Discover devices in a specific Network Interface
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <param name="deviceFoundReporter">
        ///   An implementation of <see cref="IProgress{T}" /> to handle discovered devices as soon
        ///   they are available.
        /// </param>
        /// <returns>A <see cref="Task" /> that contains a collection of all discovered <see cref="Device" /> instances.</returns>
        public static async Task<IEnumerable<Device>> DiscoverAsync(
          NetworkInterface networkInterface,
          IProgress<Device> deviceFoundReporter) =>
          await DeviceLocator.DiscoverAsync(networkInterface, deviceFoundReporter, CancellationToken.None);

        /// <summary>
        ///   Discover devices in a specific Network Interface
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to cancel the asynchronous operation.</param>
        /// <param name="deviceFoundReporter">
        ///   An implementation of <see cref="IProgress{T}" /> to handle discovered devices as soon
        ///   they are available.
        /// </param>
        /// <returns>A <see cref="Task" /> that contains a collection of all discovered <see cref="Device" /> instances.</returns>
        /// <exception cref="OperationCanceledException">Thrown when operation was cancelled by the caller.</exception>
        public static async Task<IEnumerable<Device>> DiscoverAsync(
          NetworkInterface networkInterface,
          IProgress<Device> deviceFoundReporter,
          CancellationToken cancellationToken) =>
          (await DeviceLocator.SearchNetworkForDevicesAsync(networkInterface, deviceFoundReporter, cancellationToken)).Devices;

#if NETSTANDARD2_1
        /// <summary>
        ///   Enumerate devices asynchronously.
        /// </summary>
        /// <returns>Returns an asynchronously enumerable collection of <see cref="Device" /> items.</returns>
        public static async IAsyncEnumerable<Device> EnumerateDevicesAsync()
        {
            await foreach (Device device in DeviceLocator.EnumerateDevicesAsync(CancellationToken.None))
            {
                yield return device;
            }
        }

        /// <summary>
        ///   Enumerate devices asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to cancel the asynchronous operation.</param>
        /// <returns>Returns an asynchronously enumerable collection of <see cref="Device" /> items.</returns>
        /// <exception cref="OperationCanceledException">Thrown when operation was cancelled by the caller.</exception>
        public static async IAsyncEnumerable<Device> EnumerateDevicesAsync(
          [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            IEnumerable<NetworkInterface> interfaces = NetworkInterface.GetAllNetworkInterfaces()
              .Where(networkInterface => networkInterface.OperationalStatus == OperationalStatus.Up);

            foreach (NetworkInterface networkInterface in interfaces)
            {
                cancellationToken.ThrowIfCancellationRequested();

                IEnumerable<Device> devices = await DeviceLocator.DiscoverAsync(networkInterface, cancellationToken);

                foreach (Device device in devices
                  .GroupBy(d => d.Hostname)
                  .Select(g => g.FirstOrDefault()))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return device;
                }
            }
        }

        /// <summary>
        ///   Enumerate devices asynchronously.
        /// </summary>
        /// <param name="networkInterface">The <see cref="NetworkInterface" /> of the network to search in.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to cancel the asynchronous operation.</param>
        /// <returns>Returns an asynchronously enumerable collection of <see cref="Device" /> items.</returns>
        /// <exception cref="OperationCanceledException">Thrown when operation was cancelled by the caller.</exception>
        public static async IAsyncEnumerable<Device> EnumerateDevicesAsync(
          NetworkInterface networkInterface,
          [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var aggregatedExceptions = new List<DeviceDiscoveryException>();
            int resultCount = 0;
            await foreach (DiscoveryResult discoveryResult in DeviceLocator.EnumerateNetworkInterfacesAsync(networkInterface, cancellationToken)
            )
            {
                resultCount++;

                cancellationToken.ThrowIfCancellationRequested();

                if (discoveryResult.HasError 
                    && !discoveryResult.HasResult)
                {
                    aggregatedExceptions.AddRange(discoveryResult.Exceptions);
                  continue;
                }

                if (!discoveryResult.HasResult)
                {
                    continue;
                }

                yield return discoveryResult.Devices.FirstOrDefault();
            }

            // Throw if all results failed with a SocketException
            if (aggregatedExceptions.Any() 
                && resultCount == aggregatedExceptions.Count)
            {
                throw new DeviceDiscoveryException("An error occurred during accessing a network socket. See the exception's property 'SocketExceptions' for more details.", aggregatedExceptions.SelectMany(deviceDiscoveryException => deviceDiscoveryException.SocketExceptions));
            }
        }


        /// <summary>
        ///   Create Discovery tasks for a specific Network Interface
        /// </summary>
        /// <param name="netInterface"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async IAsyncEnumerable<DiscoveryResult> EnumerateNetworkInterfacesAsync(
          NetworkInterface netInterface,
          [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (netInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
                netInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
            {
                yield break;
            }

            IEnumerable<MulticastIPAddressInformation> multicastAddresses =
              DeviceLocator.GetMulticastIPAddressesForDiscovery(netInterface.GetIPProperties().MulticastAddresses);

            cancellationToken.ThrowIfCancellationRequested();

            foreach (UnicastIPAddressInformation unicastIpAddressInformation in netInterface.GetIPProperties()
              .UnicastAddresses
              .Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork))
            {
                cancellationToken.ThrowIfCancellationRequested();

                await foreach (DiscoveryResult discoveryResult in DeviceLocator.EnumerateMulticastAddressesAsync(
                  multicastAddresses,
                  unicastIpAddressInformation,
                  cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return discoveryResult;
                }
            }
        }

        private static async IAsyncEnumerable<DiscoveryResult> EnumerateMulticastAddressesAsync(
          IEnumerable<MulticastIPAddressInformation> multicastIPAddresses,
          UnicastIPAddressInformation ip,
          [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var socketExceptions = new List<SocketException>();
            var discoveredDevicesCount = 0;
            for (var count = 0; count < DeviceLocator.MaxRetryCount; count++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                foreach (MulticastIPAddressInformation mca in multicastIPAddresses)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Socket ssdpSocket = null;

                    try
                    {
                        ssdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    }
                    catch (SocketException exception)
                    {
                        // Collect exception, just in case no devices are found after retry
                        socketExceptions.Add(exception);

                        // Ignore exception and continue with next multicast address
                        continue;
                    }

                    using (ssdpSocket)
                    {
                        var multicastIPEndpoint = new IPEndPoint(mca.Address, 1982);
                        cancellationToken.ThrowIfCancellationRequested();

                        DeviceLocator.InitializeSocket(multicastIPEndpoint, ip, ssdpSocket);
                        cancellationToken.ThrowIfCancellationRequested();

                        await foreach (Device device in DeviceLocator.EnumerateSocketDevicesAsync(
                          multicastIPEndpoint,
                          ssdpSocket,
                          socketExceptions,
                          cancellationToken))
                        {
                            discoveredDevicesCount++;
                            yield return new DiscoveryResult(new []{device}); 
                        }
                    }
                }
            }

            if (discoveredDevicesCount == 0 && socketExceptions.Any())
            {
                yield return new DiscoveryResult(
                  new[]
                  {
                    new DeviceDiscoveryException(
                      "An error occurred during accessing a network socket. See the exception's property 'SocketExceptions' for more details.",
                      socketExceptions)
                  });
            }

        }

        private static async IAsyncEnumerable<Device> EnumerateSocketDevicesAsync(
          IPEndPoint multicastIPEndpoint,
          Socket ssdpSocket,
          ICollection<SocketException> socketExceptions,
          [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ssdpSocket.SendTo(
              Encoding.ASCII.GetBytes(string.Format(DeviceLocator._ssdpMessage, multicastIPEndpoint.Address)),
              SocketFlags.None,
              multicastIPEndpoint
            );

            var stopWatch = Stopwatch.StartNew();
            Device result = null;

            // Use hash table for faster lookup, than List.Contains
            var devices = new Dictionary<string, Device>();

            try
            {
                while (stopWatch.Elapsed < TimeSpan.FromSeconds(1))
                {
                    cancellationToken.ThrowIfCancellationRequested();

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
                                cancellationToken.ThrowIfCancellationRequested();

                                Device device = DeviceLocator.GetDeviceInformationFromSsdpMessage(response);

                                if (!devices.ContainsKey(device.Hostname))
                                {
                                    devices.Add(device.Hostname, device);
                                    result = device;
                                }
                            }
                        }
                    }
                    catch (SocketException exception)
                    {
                        // Collect exception, just in case no devices are found after retry and continue polling
                        socketExceptions.Add(exception);
                    }

                    if (result != null)
                    {
                        yield return result;
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Delay(TimeSpan.FromMilliseconds(DeviceLocator.RetryDelayInMilliseconds), cancellationToken);
                }
            }
            finally
            {
                stopWatch.Stop();
            }
        }
#endif

        #endregion Async API Methods

        /// <summary>
        ///   Get the multicast addresses used by the discovery, according to the configurations
        /// </summary>
        /// <param name="multicastIPAddresses"></param>
        /// <returns></returns>
        private static IEnumerable<MulticastIPAddressInformation> GetMulticastIPAddressesForDiscovery(
          MulticastIPAddressInformationCollection multicastIPAddresses)
        {
            if (DeviceLocator.UseAllAvailableMulticastAddresses)
            {
                //return all available multicast addresses
                return multicastIPAddresses.Where(m => m.Address.AddressFamily == AddressFamily.InterNetwork);
            }

            //return default multicast address only
            MulticastIPAddressInformation multicastIpAddressesForDiscovery =
              multicastIPAddresses.FirstOrDefault(
                m => m.Address.AddressFamily == AddressFamily.InterNetwork &&
                     m.Address.Equals(IPAddress.Parse(DeviceLocator.DefaultMulticastIPAddress)));

            return multicastIpAddressesForDiscovery == null
              ? new MulticastIPAddressInformation[] { }
              : new[] { multicastIpAddressesForDiscovery };
        }

        /// <summary>
        ///   Create Discovery tasks for a specific Network Interface
        /// </summary>
        /// <param name="netInterface"></param>
        /// <param name="deviceFoundCallback"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<DiscoveryResult> SearchNetworkForDevicesAsync(
          NetworkInterface netInterface,
          IProgress<Device> deviceFoundCallback,
          CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                IEnumerable<MulticastIPAddressInformation> multicastAddresses =
                  DeviceLocator.GetMulticastIPAddressesForDiscovery(netInterface.GetIPProperties().MulticastAddresses);

                cancellationToken.ThrowIfCancellationRequested();
                ConcurrentBag<DeviceDiscoveryException> exceptions = new ConcurrentBag<DeviceDiscoveryException>();

                var allDevices = await Task.Run(
                  () => netInterface.GetIPProperties().UnicastAddresses
                    .Where(addressInfo => addressInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                    .AsParallel()
                    .WithCancellation(cancellationToken)
                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                    .SelectMany(
                      addressInformation =>
                      {
                          IEnumerable<Device> devices = new Device[0];
                          try
                          {
                              devices = DeviceLocator.CheckSocketForDevices(
                                  multicastAddresses,
                                  addressInformation,
                                  deviceFoundCallback,
                                  cancellationToken);
                          }
                          catch (DeviceDiscoveryException ex)
                          {
                              exceptions.Add(ex);
                          }

                          return devices;
                      })
                    .ToList(),
                  cancellationToken);

                return new DiscoveryResult() { Devices = allDevices, Exceptions = exceptions.ToList() };

            }

            return new DiscoveryResult() { Devices = new List<Device>(0), Exceptions = new List<DeviceDiscoveryException>(0) };
        }

        private static IEnumerable<Device> CheckSocketForDevices(
          IEnumerable<MulticastIPAddressInformation> multicastIPAddresses,
          UnicastIPAddressInformation ip,
          IProgress<Device> deviceFoundCallback,
          CancellationToken cancellationToken)
        {
            var socketExceptions = new List<SocketException>();
            IEnumerable<Device> devices = new List<Device>();
            for (var count = 0; count < DeviceLocator.MaxRetryCount; count++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                foreach (MulticastIPAddressInformation mca in multicastIPAddresses)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        using (var ssdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                        {
                            var multicastIPEndpoint = new IPEndPoint(mca.Address, 1982);
                            cancellationToken.ThrowIfCancellationRequested();

                            DeviceLocator.InitializeSocket(multicastIPEndpoint, ip, ssdpSocket);
                            cancellationToken.ThrowIfCancellationRequested();

                            devices = DeviceLocator.GetDevicesFromSocket(
                              multicastIPEndpoint,
                              deviceFoundCallback,
                              ssdpSocket,
                              socketExceptions,
                              cancellationToken);
                        }
                    }
                    catch (SocketException exception)
                    {
                        // Collect exception, just in case no devices are found after retry
                        socketExceptions.Add(exception);
                    }
                }
            }

            if (!devices.Any() && socketExceptions.Any())
            {
                throw new DeviceDiscoveryException(
                  "An error occurred during accessing a network socket. See the exception's property 'SocketExceptions' for more details.",
                  socketExceptions);
            }

            return devices;
        }

        private static void InitializeSocket(
          IPEndPoint multicastIPEndpoint,
          UnicastIPAddressInformation ip,
          Socket ssdpSocket)
        {
            ssdpSocket.Blocking = false;
            ssdpSocket.Ttl = 1;
            ssdpSocket.UseOnlyOverlappedIO = true;
            ssdpSocket.MulticastLoopback = false;
            ssdpSocket.Bind(new IPEndPoint(ip.Address, 0));
            ssdpSocket.SetSocketOption(
              SocketOptionLevel.IP,
              SocketOptionName.AddMembership,
              new MulticastOption(multicastIPEndpoint.Address));
        }

        private static ICollection<Device> GetDevicesFromSocket(
          IPEndPoint multicastIPEndpoint,
          IProgress<Device> deviceFoundCallback,
          Socket ssdpSocket,
          List<SocketException> socketExceptions,
          CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ssdpSocket.SendTo(
              Encoding.ASCII.GetBytes(string.Format(DeviceLocator._ssdpMessage, multicastIPEndpoint.Address)),
              SocketFlags.None,
              multicastIPEndpoint
            );

            var stopWatch = Stopwatch.StartNew();

            // Use hash table for faster lookup, than List.Contains
            var devices = new Dictionary<string, Device>();

            try
            {
                while (stopWatch.Elapsed < TimeSpan.FromSeconds(1))
                {
                    cancellationToken.ThrowIfCancellationRequested();

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
                                cancellationToken.ThrowIfCancellationRequested();

                                Device device = DeviceLocator.GetDeviceInformationFromSsdpMessage(response);

                                if (!devices.ContainsKey(device.Hostname))
                                {
                                    devices.Add(device.Hostname, device);
                                    deviceFoundCallback?.Report(device);
                                }
                            }
                        }
                    }
                    catch (SocketException exception)
                    {
                        // Store SocketException and continue polling
                        socketExceptions.Add(exception);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                    Thread.Sleep(TimeSpan.FromMilliseconds(DeviceLocator.RetryDelayInMilliseconds));
                }
            }
            finally
            {
                stopWatch.Stop();
            }

            return devices.Values.ToList();
        }

        /// <summary>
        ///   Gets the information from a raw SSDP message (host, port)
        /// </summary>
        /// <param name="ssdpMessage"></param>
        /// <returns></returns>
        private static Device GetDeviceInformationFromSsdpMessage(string ssdpMessage)
        {
            if (ssdpMessage != null)
            {
                string[] split = ssdpMessage.Split(new[] { Constants.LineSeparator }, StringSplitOptions.RemoveEmptyEntries);
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
                                string[] supportedOperations = propertyValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

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
    }
}