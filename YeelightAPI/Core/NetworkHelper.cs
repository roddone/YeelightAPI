using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace YeelightAPI.Core
{
    /// <summary>
    /// Helper for networking operations
    /// </summary>
    public static class NetworkHelper
    {
        /// <summary>
        /// Get the current IP adress
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIpAddress()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }

        /// <summary>
        /// Get the next available port
        /// </summary>
        /// <param name="startingPort">the port number to start searching from</param>
        /// <returns></returns>
        public static int GetNextAvailablePort(int startingPort = 0)
        {
            IPEndPoint[] endPoints;
            List<int> portArray = new List<int>();

            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

            //getting active connections
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
            if (connections.Any(c => c.LocalEndPoint.Port >= startingPort))
            {
                portArray.Add(connections.First(c => c.LocalEndPoint.Port >= startingPort).LocalEndPoint.Port);
            }

            //getting active tcp listners
            endPoints = properties.GetActiveTcpListeners();
            if (endPoints.Any(c => c.Port >= startingPort))
            {
                portArray.Add(endPoints.First(c => c.Port >= startingPort).Port);
            }

            //getting active udp listeners
            endPoints = properties.GetActiveUdpListeners();
            if (endPoints.Any(c => c.Port >= startingPort))
            {
                portArray.Add(endPoints.First(c => c.Port >= startingPort).Port);
            }

            if (portArray.Count != 0)
            {
                portArray.Sort();
                return portArray.First();
            }

            throw new Exception("No Available Port");

        }
    }
}
