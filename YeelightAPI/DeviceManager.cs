using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using YeelightClient.Models;

namespace YeelightAPI
{
    /// <summary>
    /// Yeelight Wifi Bulb Manager
    /// </summary>
    public class DeviceManager
    {
        private TcpClient tcpClient;

        private Dictionary<object, CommandResult> _currentCommandResults = new Dictionary<object, CommandResult>();

        private JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public delegate void NotificationReceivedEventHandler(object sender, NotificationReceivedEventArgs e);
        public event NotificationReceivedEventHandler NotificationReceived;

        /// <summary>
        /// Connects to a device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool Connect(string ip, int port = 55443)
        {
            this.Disconnect();

            this.tcpClient = new TcpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            this.tcpClient.Connect(endPoint);

            if (!this.tcpClient.Connected)
            {
                return false;
            }

            //continuous receiving
            Task.Factory.StartNew(() =>
            {
                while (this.tcpClient != null)
                {
                    if (this.tcpClient.Client.Available > 0)
                    {
                        byte[] bytes = new byte[this.tcpClient.Client.Available];

                        this.tcpClient.Client.Receive(bytes);

                        try
                        {
                            string datas = Encoding.UTF8.GetString(bytes);
                            CommandResult commandResult = JsonConvert.DeserializeObject<CommandResult>(datas, this._serializerSettings);

                            if (commandResult != null && commandResult.Result != null)
                            {
                                //command result
                                _currentCommandResults[commandResult.Id] = commandResult;
                            }
                            else
                            {
                                //notification result
                                NotificationResult notificationResult = JsonConvert.DeserializeObject<NotificationResult>(datas, this._serializerSettings);
                                if (notificationResult != null && notificationResult.Method != null)
                                {
                                    NotificationReceived(this, new NotificationReceivedEventArgs(notificationResult));
                                }
                            }
                        }
                        catch (Exception ex) { }
                    }
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

        /// <summary>
        /// Toggle the device power
        /// </summary>
        /// <returns></returns>
        public CommandResult Toggle()
        {
            CommandResult result = ExecuteCommandWithResponse(METHODS.Toggle);

            return result;
        }

        /// <summary>
        /// Toggle the device power
        /// </summary>
        /// <returns></returns>
        public CommandResult SetPower(bool state = true)
        {
            CommandResult result = ExecuteCommandWithResponse(method: METHODS.SetPower, parameters: new List<object>() { state ? "on" : "off" });

            return result;
        }

        /// <summary>
        /// Change the device brightness
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public CommandResult SetBrightness(int value, int? smooth = null)
        {
            CommandResult result = ExecuteCommandWithResponse(method: METHODS.SetBrightness, parameters: new List<object>() { value }, smooth: smooth);

            return result;
        }

        /// <summary>
        /// Change the device color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public CommandResult SetRGBColor(int r, int g, int b, int? smooth = null)
        {
            //Convert RGB into integer 0x00RRGGBB
            int value = ((r) << 16) | ((g) << 8) | (b);

            CommandResult result = ExecuteCommandWithResponse(method: METHODS.SetRGBColor, parameters: new List<object>() { value }, smooth: smooth);

            return result;
        }

        /// <summary>
        /// Get a single property value
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public object GetProp(string prop)
        {
            //CommandResult result = ExecCommand("get_prop", new List<string>() { $"\"{prop}\"" });
            CommandResult result = ExecuteCommandWithResponse(method: METHODS.GetProp, parameters: new List<object>() { $"{prop}" });

            return result.Result != null && result.Result.Count == 1 ? result.Result[0] : null;
        }

        /// <summary>
        /// Get multiple properties 
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetProps(List<object> props)
        {
            CommandResult commandResult = ExecuteCommandWithResponse(method: METHODS.GetProp, parameters: props);

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

        /// <summary>
        /// Execute a command and waits for a response during 1 second
        /// </summary>
        /// <param name="method"></param>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public CommandResult ExecuteCommandWithResponse(METHODS method, int id = 0, List<object> parameters = null, int? smooth = null)
        {
            if (this._currentCommandResults.ContainsKey(id))
            {
                this._currentCommandResults.Remove(id);
            }

            ExecuteCommand(method, id, parameters, smooth);

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
        /// Execute a command
        /// </summary>
        /// <param name="method"></param>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <param name="smooth"></param>
        public void ExecuteCommand(METHODS method, int id = 0, List<object> parameters = null, int? smooth = null)
        {
            Command command = new Command()
            {
                Id = id,
                Method = method.GetRealName(),
                Params = parameters ?? new List<object>()
            };

            if (smooth.HasValue)
            {
                command.Params.Add("smooth");
                command.Params.Add(smooth);
            }
            string data = JsonConvert.SerializeObject(command, this._serializerSettings);
            byte[] sentData = Encoding.ASCII.GetBytes(data + "\r\n"); // \r\n is the end of the message, it needs to be sent for the message to be read by the device

            this.tcpClient.Client.Send(sentData);
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
                         .Where(m => properties.HasFlag(m) && m != PROPERTIES.All)
                         .Cast<PROPERTIES>()
                         .Select(x => x.GetRealName())
                         .ToList<object>();
        }
    }
}
