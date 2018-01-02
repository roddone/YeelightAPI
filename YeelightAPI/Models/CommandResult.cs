using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightClient.Models
{
    /// <summary>
    /// Result received after a Command has been sent
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// Request Id (mirrored from the sent request)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Error, null if command is successful
        /// </summary>
        public CommandErrorResult Error { get; set; }

        /// <summary>
        /// Result
        /// </summary>
        public List<string> Result { get; set; }

        /// <summary>
        /// Error model
        /// </summary>
        public class CommandErrorResult
        {
            /// <summary>
            /// Error code
            /// </summary>
            public int Code { get; set; }

            /// <summary>
            /// Error message
            /// </summary>
            public string Message { get; set; }
        }
    }

}
