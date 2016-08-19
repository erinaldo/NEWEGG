using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISearchProperty class.
    /// </summary>
    [DataContract]
    public class UISearchProperty
    {
        /// <summary>
        /// Initializes a new instance of the UISearchProperty class.
        /// </summary>
        public UISearchProperty()
        {            
            this.Page = 1;
            this.ShowAll = false;
        }

        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }
        
        /// <summary>
        /// Gets or sets Keyword.
        /// </summary>
        [DataMember(Name = "Keyword")]
        public string Keyword { get; set; }
        
        /// <summary>
        /// Gets or sets Option.
        /// </summary>
        [DataMember(Name = "Option")]
        public string Option { get; set; }
        
        /// <summary>
        /// Gets or sets BeginDate.
        /// </summary>
        [DataMember(Name = "BeginDate")]
        public DateTime BeginDate { get; set; }
        
        /// <summary>
        /// Gets or sets EndDate.
        /// </summary>
        [DataMember(Name = "EndDate")]
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Gets or sets Page.
        /// </summary>
        [DataMember(Name = "Page")]
        public int Page { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether ShowAll.
        /// </summary>
        [DataMember(Name = "ShowAll")]
        public bool ShowAll { get; set; }       
    }
}
