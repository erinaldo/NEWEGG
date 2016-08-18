using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Error Code Mapping.
    /// </summary>
    public class ErrorCode
    {
        /// <summary>
        /// No Error.
        /// </summary>
        public const string NoError = "000";

        /// <summary>
        /// Service invoke Error.
        /// </summary>
        public const string Error = "222";

        /// <summary>
        /// Server Logic Error.
        /// </summary>
        public const string LogicError = "111";

        /// <summary>
        /// Has prompt.
        /// </summary>
        public const string HasPrompt = "999";
    }
}
