using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using YeelightAPI.Models;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Cron;

namespace YeelightAPI
{
    /// <summary>
    /// Yeelight Device : IDeviceController implementation
    /// </summary>
    public partial class Device : IDeviceController
    {
        /// <summary>
        /// Disconnect the current device
        /// </summary>
        /// <returns></returns>
        public void Disconnect()
        {
            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient = null;
            }
        }

        /// <summary>
        /// Connects to a device asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Connect()
        {
            Disconnect();

            _tcpClient = new TcpClient();
            //IPEndPoint endPoint = GetIPEndPointFromHostName(this.Hostname, this.Port);
            await _tcpClient.ConnectAsync(Hostname, Port);

            if (!_tcpClient.Connected)
            {
                return false;
            }

            //continuous receiving
#pragma warning disable 4014
            Watch();
#pragma warning restore 4014

            //initialiazing all properties
            foreach (KeyValuePair<PROPERTIES, object> property in await GetAllProps())
            {
                this[property.Key] = property.Value;
            }

            return true;
        }

        /// <summary>
        /// Toggle the device power asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Toggle()
        {
            CommandResult result = await ExecuteCommandWithResponse(METHODS.Toggle, id: (int)METHODS.Toggle);

            return result.IsOk();
        }

        /// <summary>
        /// Set the device power state asynchronously
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<bool> SetPower(bool state = true)
        {
            CommandResult result = await ExecuteCommandWithResponse(
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
        public async Task<bool> SetBrightness(int value, int? smooth = null)
        {
            List<object> parameters = new List<object>() { value };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponse(
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
        public async Task<bool> SetRGBColor(int r, int g, int b, int? smooth = null)
        {
            //Convert RGB into integer 0x00RRGGBB
            int value = ((r) << 16) | ((g) << 8) | (b);
            List<object> parameters = new List<object>() { value };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponse(
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
        public async Task<bool> SetColorTemperature(int temperature, int? smooth = null)
        {
            List<object> parameters = new List<object>() { temperature };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponse(
                method: METHODS.SetColorTemperature,
                id: (int)METHODS.SetColorTemperature,
                parameters: parameters);

            return result.IsOk();
        }

        /// <summary>
        /// Change HSV color asynchronously
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="sat"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetHSVColor(int hue, int sat, int? smooth = null)
        {
            List<object> parameters = new List<object>() { hue, sat };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponse(
                method: METHODS.SetHSVColor,
                id: (int)METHODS.SetHSVColor,
                parameters: parameters);

            return result.IsOk();
        }

        /// <summary>
        /// Starts a color flow asynchronously
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="action"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<bool> StartColorFlow(ColorFlow flow)
        {
            List<object> parameters = new List<object>() { flow.RepetitionCount, (int)flow.EndAction, flow.GetColorFlowExpression() };

            CommandResult result = await ExecuteCommandWithResponse(
                method: METHODS.StartColorFlow,
                id: (int)METHODS.StartColorFlow,
                parameters: parameters);

            return result.IsOk();
        }

        /// <summary>
        /// Stops the color flow
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StopColorFlow()
        {
            CommandResult result = await ExecuteCommandWithResponse(
                            method: METHODS.StopColorFlow,
                            id: (int)METHODS.StopColorFlow);

            return result.IsOk();
        }

        /// <summary>
        /// Adjusts the state of the device
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public async Task<bool> SetAdjust(AdjustAction action, AdjustProperty property)
        {
            List<object> parameters = new List<object>() { action.ToString(), property.ToString() };

            CommandResult result = await ExecuteCommandWithResponse(
                method: METHODS.SetAdjust,
                id: (int)METHODS.SetAdjust,
                parameters: parameters);

            return result.IsOk();
        }

        /// <summary>
        /// Add a cron job
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<bool> CronAdd(int value, CronType type = CronType.PowerOff)
        {
            List<object> parameters = new List<object>() { (int)type, value };

            CommandResult result = await ExecuteCommandWithResponse(
                method: METHODS.AddCron,
                id: (int)METHODS.AddCron,
                parameters: parameters);

            return result.IsOk();
        }

        /// <summary>
        /// Delete a cron job
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<bool> CronDelete(CronType type = CronType.PowerOff)
        {
            List<object> parameters = new List<object>() { (int)type };

            CommandResult result = await ExecuteCommandWithResponse(
                method: METHODS.DeleteCron,
                id: (int)METHODS.DeleteCron,
                parameters: parameters);

            return result.IsOk();
        }
    }
}