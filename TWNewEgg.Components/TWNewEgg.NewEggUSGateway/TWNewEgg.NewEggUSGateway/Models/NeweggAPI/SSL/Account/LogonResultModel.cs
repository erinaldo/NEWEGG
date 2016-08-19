using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the LogonResultModel class.
    /// </summary>
    [DataContract]
    public class LogonResultModel
    {
        /// <summary>
        /// Gets or sets LogonResult.
        /// </summary>
        [DataMember(Name = "LogonResult")]
        public ResultStatus LogonResult { get; set; }

        /// <summary>
        /// Gets or sets ErrorMessage.
        /// </summary>
        [DataMember(Name = "ErrorMessage")]
        public string ErrorMessage { get; set; }
    }
}
