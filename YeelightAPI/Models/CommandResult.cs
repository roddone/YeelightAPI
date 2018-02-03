using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightAPI.Models
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

            /// <summary>
            /// ToString override
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"{this.Code} - {this.Message}";
            }
        }
    }

    /// <summary>
    /// Extensions for CommandResult
    /// </summary>
    public static class CommandResultExtensions
    {
        /// <summary>
        /// Determine if the result is a classical OK result ({"id":1, "result":["ok"]})
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool IsOk(this CommandResult @this)
        {
            return @this != null && @this.Error == null && @this.Result?[0] == "ok";
        }
    }

}
