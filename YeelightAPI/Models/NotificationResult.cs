using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightAPI.Models
{
    /// <summary>
    /// Notification, resulting of a command which has changed the state of the bulb
    /// </summary>
    public class NotificationResult
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Parameters
        /// </summary>
        public Dictionary<string, string> Params { get; set; }

    }
}
