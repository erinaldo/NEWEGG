using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICustomerReviewInfo class.
    /// </summary>
    [DataContract]
    public class UICustomerReviewInfo
    {
        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }
        
        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [DataMember(Name = "Title")]
        public string Title { get; set; }
        
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }
        
        /// <summary>
        /// Gets or sets LoginName.
        /// </summary>
        [DataMember(Name = "LoginName")]
        public string LoginName { get; set; }
        
        /// <summary>
        /// Gets or sets NickName.
        /// </summary>
        [DataMember(Name = "NickName")]
        public string NickName { get; set; }
        
        /// <summary>
        /// Gets or sets Rating.
        /// </summary>
        [DataMember(Name = "Rating")]
        public int Rating { get; set; }
        
        /// <summary>
        /// Gets or sets BoughtTime.
        /// </summary>
        [DataMember(Name = "BoughtTime")]
        public int BoughtTime { get; set; }
        
        /// <summary>
        /// Gets or sets TechLevel.
        /// </summary>
        [DataMember(Name = "TechLevel")]
        public int TechLevel { get; set; }
        
        /// <summary>
        /// Gets or sets Pros.
        /// </summary>
        [DataMember(Name = "Pros")]
        public string Pros { get; set; }
        
        /// <summary>
        /// Gets or sets Cons.
        /// </summary>
        [DataMember(Name = "Cons")]
        public string Cons { get; set; }
        
        /// <summary>
        /// Gets or sets Comments.
        /// </summary>
        [DataMember(Name = "Comments")]
        public string Comments { get; set; }
        
        /// <summary>
        /// Gets or sets UploadID.
        /// </summary>
        [DataMember(Name = "UploadID")]
        public string UploadID { get; set; }
        
        /// <summary>
        /// Gets or sets IPAddress.
        /// </summary>
        [DataMember(Name = "IPAddress")]
        public string IPAddress { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether HasVideo.
        /// </summary>
        [DataMember(Name = "HasVideo")]
        public bool HasVideo { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether IsForVideoGameItem.
        /// </summary>
        [DataMember(Name = "IsForVideoGameItem")]
        public bool IsForVideoGameItem { get; set; }
        
        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [DataMember(Name = "Type")]
        public string Type { get; set; }
        
        /// <summary>
        /// Gets or sets OrderNumber.
        /// </summary>
        [DataMember(Name = "OrderNumber")]
        public int OrderNumber { get; set; }
        
        /// <summary>
        /// Gets or sets OrderDate.
        /// </summary>
        [DataMember(Name = "OrderDate")]
        public DateTime OrderDate { get; set; }
        
        /// <summary>
        /// Gets or sets InDate.
        /// </summary>
        [DataMember(Name = "InDate")]
        public DateTime InDate { get; set; }
    }
}
