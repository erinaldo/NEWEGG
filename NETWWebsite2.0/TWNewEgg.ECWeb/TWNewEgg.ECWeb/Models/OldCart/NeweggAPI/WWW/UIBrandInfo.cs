using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIBrandInfo class.
    /// </summary>
    [DataContract]
    public class UIBrandInfo
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        [DataMember(Name = "Code")]
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets BrandId.
        /// </summary>
        [DataMember(Name = "BrandId")]
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets ManufactoryWeb.
        /// </summary>
        [DataMember(Name = "ManufactoryWeb")]
        public string ManufactoryWeb { get; set; }

        /// <summary>
        /// Gets or sets WebSiteURL.
        /// </summary>
        [DataMember(Name = "WebSiteURL")]
        public string WebSiteURL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HasManfactoryLogo.
        /// </summary>
        [DataMember(Name = "HasManfactoryLogo")]
        public bool HasManfactoryLogo { get; set; }

        /// <summary>
        /// Gets or sets BrandImage.
        /// </summary>
        [DataMember(Name = "BrandImage")]
        public string BrandImage { get; set; }
    }
}
