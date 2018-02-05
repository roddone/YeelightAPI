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
    public partial class Device : IDisposable
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
        public event NotificationReceivedEventHandler OnNotificationReceived;

        /// <summary>
        /// Error Received event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void CommandErrorEventHandler(object sender, CommandErrorEventArgs e);

        /// <summary>
        /// Error Received event
        /// </summary>
        public event CommandErrorEventHandler OnCommandError;

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
                                            OnCommandError?.Invoke(this, new CommandErrorEventArgs(commandResult.Error));
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
                                                OnNotificationReceived?.Invoke(this, new NotificationReceivedEventArgs(notificationResult));
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
