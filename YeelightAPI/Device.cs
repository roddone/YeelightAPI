﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YeelightAPI.Models;

namespace YeelightAPI
{
    /// <summary>
    /// Yeelight Wifi Bulb Manager
    /// </summary>
    public class Device : IDeviceController, IDeviceReader
    {

        #region PRIVATE ATTRIBUTES

        private object _syncLock = new object();

        private TcpClient tcpClient;

        private Dictionary<object, CommandResult> _currentCommandResults = new Dictionary<object, CommandResult>();

        private JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        #endregion PRIVATE ATTRIBUTES

        #region EVENTS

        public delegate void NotificationReceivedEventHandler(object sender, NotificationReceivedEventArgs e);
        public event NotificationReceivedEventHandler NotificationReceived;

        #endregion EVENTS

        #region PUBLIC PROPERTIES

        /// <summary>
        /// HostName
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Port number
        /// </summary>
        public int Port { get; set; }

        #endregion PUBLIC PROPERTIES

        #region CONSTRUCTOR

        /// <summary>
        /// Constructor with a hostname and (optionally) a port number
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        public Device(string hostname, int port = Common.DefaultPort)
        {
            this.Hostname = hostname;
            this.Port = port;
        }

        #endregion CONSTRUCTOR

        #region PUBLIC METHODS

        /// <summary>
        /// Connects to a device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool Connect()
        {
            this.Disconnect();

            this.tcpClient = new TcpClient();
            IPEndPoint endPoint = GetIPEndPointFromHostName(this.Hostname, this.Port);
            this.tcpClient.Connect(endPoint);

            if (!this.tcpClient.Connected)
            {
                return false;
            }

            //continuous receiving
            Task.Factory.StartNew(async () =>
            {
                while (this.tcpClient != null)
                {
                    lock (_syncLock)
                    {
                        if (this.tcpClient.Client.Available > 0)
                        {
                            byte[] bytes = new byte[this.tcpClient.Client.Available];

                            this.tcpClient.Client.Receive(bytes);

                            try
                            {
                                string datas = Encoding.UTF8.GetString(bytes);
                                if (!string.IsNullOrEmpty(datas))
                                {
                                    foreach (string entry in datas.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        CommandResult commandResult = JsonConvert.DeserializeObject<CommandResult>(entry, this._serializerSettings);

                                        if (commandResult != null && (commandResult.Result != null || commandResult.Error != null))
                                        {
                                            //command result
                                            _currentCommandResults[commandResult.Id] = commandResult;
                                        }
                                        else
                                        {
                                            //notification result
                                            NotificationResult notificationResult = JsonConvert.DeserializeObject<NotificationResult>(entry, this._serializerSettings);
                                            if (notificationResult != null && notificationResult.Method != null && NotificationReceived != null)
                                            {
                                                NotificationReceived(this, new NotificationReceivedEventArgs(notificationResult));
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error while reading through pipe : {ex.Message}");
                            }
                        }
                    }
                    await Task.Delay(100);
                }
            }, TaskCreationOptions.LongRunning);

            return true;
        }

        /// <summary>
        /// Disconnect the current device
        /// </summary>
        /// <returns></returns>
        public void Disconnect()
        {
            if (this.tcpClient != null)
            {
                this.tcpClient.Close();
                this.tcpClient = null;
            }
        }

        #region IDeviceController
        #region synchrone

        /// <summary>
        /// Toggle the device power
        /// </summary>
        /// <returns></returns>
        public bool Toggle()
        {
            CommandResult result = ExecuteCommandWithResponse(METHODS.Toggle, id: (int)METHODS.Toggle);

            return result.IsOk;
        }

        /// <summary>
        /// Set the device power state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool SetPower(bool state = true)
        {
            CommandResult result = ExecuteCommandWithResponse(
                method: METHODS.SetPower,
                id: (int)METHODS.SetPower,
                parameters: new List<object>() { state ? "on" : "off" }
            );

            return result.IsOk;
        }

        /// <summary>
        /// Change the device brightness
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public bool SetBrightness(int value, int? smooth = null)
        {
            List<object> parameters = new List<object>() { value };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = ExecuteCommandWithResponse(
                method: METHODS.SetBrightness,
                id: (int)METHODS.SetBrightness,
                parameters: parameters);

            return result.IsOk;
        }

        /// <summary>
        /// Change the device RGB color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public bool SetRGBColor(int r, int g, int b, int? smooth)
        {
            //Convert RGB into integer 0x00RRGGBB
            int value = ((r) << 16) | ((g) << 8) | (b);
            List<object> parameters = new List<object>() { value };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = ExecuteCommandWithResponse(
                method: METHODS.SetRGBColor,
                id: (int)METHODS.SetRGBColor,
                parameters: parameters);

            return result.IsOk;
        }

        /// <summary>
        /// Change Color Temperature
        /// </summary>
        /// <param name="saturation"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public bool SetColorTemperature(int temperature, int? smooth)
        {
            List<object> parameters = new List<object>() { temperature };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = ExecuteCommandWithResponse(
                method: METHODS.SetColorTemperature,
                id: (int)METHODS.SetColorTemperature,
                parameters: parameters);

            return result.IsOk;
        }

        #endregion synchrone

        #region asynchrone

        /// <summary>
        /// Toggle the device power asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ToggleAsync()
        {
            CommandResult result = await ExecuteCommandWithResponseAsync(METHODS.Toggle, id: (int)METHODS.Toggle);

            return result.IsOk;
        }

        /// <summary>
        /// Set the device power state asynchronously
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<bool> SetPowerAsync(bool state = true)
        {
            CommandResult result = await ExecuteCommandWithResponseAsync(
                method: METHODS.SetPower,
                id: (int)METHODS.SetPower,
                parameters: new List<object>() { state ? "on" : "off" }
            );

            return result.IsOk;
        }

        /// <summary>
        /// Change the device brightness asynchronously
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetBrightnessAsync(int value, int? smooth = null)
        {
            List<object> parameters = new List<object>() { value };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponseAsync(
                method: METHODS.SetBrightness,
                id: (int)METHODS.SetBrightness,
                parameters: parameters);

            return result.IsOk;
        }

        /// <summary>
        /// Change the device RGB color asynchronously
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetRGBColorAsync(int r, int g, int b, int? smooth)
        {
            //Convert RGB into integer 0x00RRGGBB
            int value = ((r) << 16) | ((g) << 8) | (b);
            List<object> parameters = new List<object>() { value };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponseAsync(
                method: METHODS.SetRGBColor,
                id: (int)METHODS.SetRGBColor,
                parameters: parameters);

            return result.IsOk;
        }

        /// <summary>
        /// Change Color Temperature asynchronously
        /// </summary>
        /// <param name="saturation"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetColorTemperatureAsync(int temperature, int? smooth)
        {
            List<object> parameters = new List<object>() { temperature };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponseAsync(
                method: METHODS.SetColorTemperature,
                id: (int)METHODS.SetColorTemperature,
                parameters: parameters);

            return result.IsOk;
        }

        #endregion asynchrone
        #endregion IDeviceController

        #region IDeviceReader

        /// <summary>
        /// Get a single property value
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public object GetProp(string prop)
        {
            //CommandResult result = ExecCommand("get_prop", new List<string>() { $"\"{prop}\"" });
            CommandResult result = ExecuteCommandWithResponse(
                method: METHODS.GetProp,
                id: (int)METHODS.GetProp,
                parameters: new List<object>() { prop.ToString() }
                );

            return result.Result != null && result.Result.Count == 1 ? result.Result[0] : null;
        }

        /// <summary>
        /// Get multiple properties 
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetProps(List<object> props)
        {
            CommandResult commandResult = ExecuteCommandWithResponse(
                method: METHODS.GetProp,
                id: ((int)METHODS.GetProp) + 1000 + props.Count,
                parameters: props
                );

            Dictionary<string, object> result = new Dictionary<string, object>();

            for (int p = 0; p < props.Count; p++)
            {
                result.Add(props[p].ToString(), commandResult.Result[p]);
            }

            return result;
        }

        /// <summary>
        /// Get all the properties
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetAllProps()
        {
            List<object> names = GetPropertiesRealNames(PROPERTIES.All);
            Dictionary<string, object> result = GetProps(names);

            return result;
        }

        #endregion IDeviceReader

        /// <summary>
        /// Execute a command and waits for a response during 1 second
        /// </summary>
        /// <param name="method"></param>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public CommandResult ExecuteCommandWithResponse(METHODS method, int id = 0, List<object> parameters = null)
        {
            if (this._currentCommandResults.ContainsKey(id))
            {
                this._currentCommandResults.Remove(id);
            }

            ExecuteCommand(method, id, parameters);

            DateTime startWait = DateTime.Now;
            while (!this._currentCommandResults.ContainsKey(id) && DateTime.Now - startWait < TimeSpan.FromSeconds(1)) { }//attente du prochain résultat, laisse tomber au bout de 1 seconde

            //sauvegarde du résultat et on le retire de la liste des résultats en attente de traitement
            if (this._currentCommandResults.ContainsKey(id))
            {
                CommandResult result = this._currentCommandResults[id];
                this._currentCommandResults.Remove(id);

                return result;
            }

            return null;
        }

        /// <summary>
        /// Execute a command and waits for a response during 1 second
        /// </summary>
        /// <param name="method"></param>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<CommandResult> ExecuteCommandWithResponseAsync(METHODS method, int id = 0, List<object> parameters = null)
        {
            if (this._currentCommandResults.ContainsKey(id))
            {
                this._currentCommandResults.Remove(id);
            }

            ExecuteCommand(method, id, parameters);

            await Task.Factory.StartNew(() =>
            {
                DateTime startWait = DateTime.Now;
                while (!this._currentCommandResults.ContainsKey(id) && DateTime.Now - startWait < TimeSpan.FromSeconds(1)) { }//attente du prochain résultat, laisse tomber au bout de 1 seconde
            });

            //sauvegarde du résultat et on le retire de la liste des résultats en attente de traitement
            if (this._currentCommandResults.ContainsKey(id))
            {
                CommandResult result = this._currentCommandResults[id];
                this._currentCommandResults.Remove(id);

                return result;
            }

            return null;
        }

        /// <summary>
        /// Execute a command
        /// </summary>
        /// <param name="method"></param>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <param name="smooth"></param>
        public void ExecuteCommand(METHODS method, int id = 0, List<object> parameters = null)
        {
            Command command = new Command()
            {
                Id = id,
                Method = method.GetRealName(),
                Params = parameters ?? new List<object>()
            };

            string data = JsonConvert.SerializeObject(command, this._serializerSettings);
            byte[] sentData = Encoding.ASCII.GetBytes(data + "\r\n"); // \r\n is the end of the message, it needs to be sent for the message to be read by the device

            lock (_syncLock)
            {
                this.tcpClient.Client.Send(sentData);
            }
        }

        #endregion PUBLIC METHODS

        #region PRIVATE METHODS

        /// <summary>
        /// Get the real name of the properties
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private static List<object> GetPropertiesRealNames(PROPERTIES properties)
        {
            var vals = Enum.GetValues(typeof(PROPERTIES));
            return vals
                         .Cast<PROPERTIES>()
                         .Where(m => properties.HasFlag(m) && m != PROPERTIES.All)
                         .Cast<PROPERTIES>()
                         .Select(x => x.GetRealName())
                         .ToList<object>();
        }

        /// <summary>
        /// Get an ip from a hostname
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private static IPEndPoint GetIPEndPointFromHostName(string hostName, int port)
        {
            var addresses = System.Net.Dns.GetHostAddresses(hostName);
            if (addresses.Length == 0)
            {
                throw new ArgumentException(
                    "Unable to retrieve address from specified host name.",
                    "hostName"
                );
            }
            else if (addresses.Length > 1)
            {
                throw new ArgumentException(
                    "There is more that one IP address to the specified host.",
                    "hostName"
                );
            }
            return new IPEndPoint(addresses[0], port); // Port gets validated here.
        }

        /// <summary>
        /// Generate valid parameters for smooth values
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="smooth"></param>
        private static void HandleSmoothValue(ref List<object> parameters, int? smooth)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (smooth.HasValue)
            {
                parameters.Add("smooth");
                parameters.Add(smooth.Value);
            }
            else
            {
                parameters.Add("sudden");
                parameters.Add(null); // two parameters needed
            }
        }

        #endregion PRIVATE METHODS
    }
}
