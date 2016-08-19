using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the ThirdPartyContextBase class.
    /// </summary>
    public interface IThirdPartyContext
    {
        /// <summary>
        /// Gets or sets ThirdPartyContext.
        /// </summary>
        ThirdPartyContext ThirdPartyContext { get; set; }
    }
}
