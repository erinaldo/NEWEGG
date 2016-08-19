using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIImageInfo class.
    /// </summary>
    [DataContract]
    public class UIImageInfo
    {
        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [DataMember(Name = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets FullPath.
        /// </summary>
        [DataMember(Name = "FullPath")]
        public string FullPath { get; set; }

        /// <summary>
        /// Gets or sets PathSize35.
        /// </summary>
        [DataMember(Name = "PathSize35")]
        public string PathSize35 { get; set; }

        /// <summary>
        /// Gets or sets PathSize60.
        /// </summary>
        [DataMember(Name = "PathSize60")]
        public string PathSize60 { get; set; }

        /// <summary>
        /// Gets or sets PathSize100.
        /// </summary>
        [DataMember(Name = "PathSize100")]
        public string PathSize100 { get; set; }

        /// <summary>
        /// Gets or sets PathSize125.
        /// </summary>
        [DataMember(Name = "PathSize125")]
        public string PathSize125 { get; set; }

        /// <summary>
        /// Gets or sets PathSize180.
        /// </summary>
        [DataMember(Name = "PathSize180")]
        public string PathSize180 { get; set; }

        /// <summary>
        /// Gets or sets PathSize300.
        /// </summary>
        [DataMember(Name = "PathSize300")]
        public string PathSize300 { get; set; }

        /// <summary>
        /// Gets or sets PathSize640.
        /// </summary>
        [DataMember(Name = "PathSize640")]
        public string PathSize640 { get; set; }
    }
}