using System;
using System.Collections.Generic;
using System.Text;

namespace YeelightAPI.Models.Cron
{
    /// <summary>
    /// Cron result
    /// </summary>
    public class CronResult
    {
        /// <summary>
        /// Type of the cron task
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        public int Mix { get; set; }
    }
}
