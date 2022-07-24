using System;
using System.Collections.Generic;
using System.Text;

namespace YeelightAPI.Core
{
    /// <summary>
    /// Informations about device music mode 
    /// </summary>
    public class MusicModeInformations
    {
        /// <summary>
        /// The host name
        /// </summary>
        public string HostName { get; internal set; }
        
        /// <summary>
        /// The used port
        /// </summary>
        public int Port { get; internal set; }
        
        /// <summary>
        /// Music mode enabled or not ?
        /// </summary>
        public bool Enabled { get; internal set; }

        /// <summary>
        /// High rate enabled or not ?
        /// </summary>
        public bool HighRateEnabled { get; set; }
    }
}
