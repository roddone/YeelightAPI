using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
    /// Yeelight Wifi Device Manager
    /// </summary>
    public class Device : IDeviceController, IDeviceReader, IDisposable
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

        /// <summary>
        /// Notification Received event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void NotificationReceivedEventHandler(object sender, NotificationReceivedEventArgs e);

        /// <summary>
        /// Notification Received event
        /// </summary>
        public event NotificationReceivedEventHandler NotificationReceived;

        /// <summary>
        /// Error Received event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

        /// <summary>
        /// Error Received event
        /// </summary>
        public event ErrorEventHandler OnCommandError;

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

        /// <summary>
        /// Name of the device
        /// </summary>
        public string Name { get; set; }

        #endregion PUBLIC PROPERTIES

        #region CONSTRUCTOR

        /// <summary>
        /// Constructor with a hostname and (optionally) a port number
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        public Device(string hostname, int port = Common.DefaultPort, string name = null, bool autoConnect = false)
        {
            this.Hostname = hostname;
            this.Port = port;
            this.Name = name;

            //autoconnect device if specified
            if (autoConnect)
            {
                this.Connect();
            }
        }

        #endregion CONSTRUCTOR

        #region PROPERTIES ACCESS

        /// <summary>
        /// List of device properties
        /// </summary>
        public Dictionary<string, object> Properties = new Dictionary<string, object>();

        /// <summary>
        /// Access property from its enum value
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public object this[PROPERTIES property]
        {
            get
            {
                return this[property.ToString()];
            }
            set
            {
                this[property.ToString()] = value;
            }
        }

        /// <summary>
        /// Access property from its name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object this[string propertyName]
        {
            get
            {
                if (this.Properties.ContainsKey(propertyName))
                {
                    return this.Properties[propertyName];
                }
                return null;
            }
            set
            {
                if (this.Properties.ContainsKey(propertyName))
                {
                    this.Properties[propertyName] = value;
                }
                else if (!string.IsNullOrWhiteSpace(propertyName))
                {
                    this.Properties.Add(propertyName, value);
                }
            }
        }

        #endregion PROPERTIES ACCESS

        #region PUBLIC METHODS

        #region IDeviceController

        /// <summary>
        /// Connects to a device
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            return ConnectAsync().Result;
        }

        /// <summary>
        /// Connects to a device asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ConnectAsync()
        {
            this.Disconnect();

            this.tcpClient = new TcpClient();
            //IPEndPoint endPoint = GetIPEndPointFromHostName(this.Hostname, this.Port);
            await this.tcpClient.ConnectAsync(this.Hostname, this.Port);

            if (!this.tcpClient.Connected)
            {
                return false;
            }

            //continuous receiving
#pragma warning disable 4014
            this.Watch();
#pragma warning restore 4014

            //initialiazing all properties
            foreach (KeyValuePair<PROPERTIES, object> property in this.GetAllProps())
            {
                this[property.Key] = property.Value;
            }

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
                this.tcpClient.Dispose();
                this.tcpClient = null;
            }
        }

        #region synchrone

        /// <summary>
        /// Toggle the device power
        /// </summary>
        /// <returns></returns>
        public bool Toggle()
        {
            CommandResult result = ExecuteCommandWithResponse(METHODS.Toggle, id: (int)METHODS.Toggle);

            return result.IsOk();
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

            return result.IsOk();
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

            return result.IsOk();
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

            return result.IsOk();
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

            return result.IsOk();
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

            return result.IsOk();
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

            return result.IsOk();
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

            return result.IsOk();
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

            return result.IsOk();
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

            return result.IsOk();
        }

        #endregion asynchrone
        #endregion IDeviceController

        #region IDeviceReader

        /// <summary>
        /// Get a single property value
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public object GetProp(PROPERTIES prop)
        {
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
        public Dictionary<PROPERTIES, object> GetProps(PROPERTIES props)
        {
            List<object> names = GetPropertiesRealNames(props);

            CommandResult commandResult = ExecuteCommandWithResponse(
                method: METHODS.GetProp,
                id: ((int)METHODS.GetProp),// + 1000 + props.Count,
                parameters: names
                );

            Dictionary<PROPERTIES, object> result = new Dictionary<PROPERTIES, object>();

            for (int n = 0; n < names.Count; n++)
            {
                string name = names[n].ToString();

                if (Enum.TryParse<PROPERTIES>(name, out PROPERTIES p))
                {
                    result.Add(p, commandResult.Result[n]);
                }
            }

            return result;
        }

        /// <summary>
        /// Get all the properties
        /// </summary>
        /// <returns></returns>
        public Dictionary<PROPERTIES, object> GetAllProps()
        {
            Dictionary<PROPERTIES, object> result = GetProps(PROPERTIES.ALL);

            return result;
        }

        #endregion IDeviceReader

        #region IDisposable

        /// <summary>
        /// Dispose the device
        /// </summary>
        public void Dispose()
        {
            this.Disconnect();
        }

        #endregion IDisposable

        /// <summary>
        /// Execute a command and waits for a response during 5 seconds
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
            while (!this._currentCommandResults.ContainsKey(id) && DateTime.Now - startWait < TimeSpan.FromSeconds(5)) { } //wait for result during 1s

            //save results and remove if from results list
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
                while (!this._currentCommandResults.ContainsKey(id) && DateTime.Now - startWait < TimeSpan.FromSeconds(1)) { } //wait for result during 1s
            });

            //save results and remove if from results list
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
        /// Watch for device responses and notifications
        /// </summary>
        /// <returns></returns>
        private async Task Watch()
        {
            await Task.Factory.StartNew(async () =>
            {
                //while device is connected
                while (this.tcpClient != null)
                {
                    lock (_syncLock)
                    {
                        //there is data avaiblable in the pipe
                        if (this.tcpClient.Client.Available > 0)
                        {
                            byte[] bytes = new byte[this.tcpClient.Client.Available];

                            //read datas
                            this.tcpClient.Client.Receive(bytes);

                            try
                            {
                                string datas = Encoding.UTF8.GetString(bytes);
                                if (!string.IsNullOrEmpty(datas))
                                {
                                    //get every messages in the pipe
                                    foreach (string entry in datas.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        CommandResult commandResult = JsonConvert.DeserializeObject<CommandResult>(entry, this._serializerSettings);

                                        if (commandResult != null && (commandResult.Result != null || commandResult.Error != null))
                                        {
                                            //command result
                                            _currentCommandResults[commandResult.Id] = commandResult;
                                        }
                                        else if (commandResult != null && commandResult.Error != null)
                                        {
                                            //error result
                                            OnCommandError?.Invoke(this, new ErrorEventArgs(commandResult.Error));
                                        }
                                        else
                                        {
                                            NotificationResult notificationResult = JsonConvert.DeserializeObject<NotificationResult>(entry, this._serializerSettings);

                                            if (notificationResult != null && notificationResult.Method != null)
                                            {
                                                if (notificationResult.Params != null)
                                                {
                                                    //save properties
                                                    foreach (KeyValuePair<PROPERTIES, object> property in notificationResult.Params)
                                                    {
                                                        this[property.Key] = property.Value;
                                                    }
                                                }

                                                //notification result
                                                NotificationReceived?.Invoke(this, new NotificationReceivedEventArgs(notificationResult));
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
        }

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
                         .Where(m => properties.HasFlag(m) && m != PROPERTIES.ALL && m != PROPERTIES.NONE)
                         .Cast<PROPERTIES>()
                         .Select(x => x.ToString())
                         .ToList<object>();
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
