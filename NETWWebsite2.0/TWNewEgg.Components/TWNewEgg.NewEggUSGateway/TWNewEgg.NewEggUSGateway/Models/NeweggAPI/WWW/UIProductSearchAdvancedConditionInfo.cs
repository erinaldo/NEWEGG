using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductSearchAdvancedConditionInfo class.
    /// </summary>
    [DataContract]
    public class UIProductSearchAdvancedConditionInfo : UIProductSearchCondition
    {
        /// <summary>
        /// Initializes a new instance of the UIProductSearchAdvancedConditionInfo class.
        /// </summary>
        public UIProductSearchAdvancedConditionInfo()
        {
            this.BrandId = -1;
            this.SubCategoryId = -1;
            this.StoreDepaId = -1;
            this.CategoryId = -1;
            this.NodeId = -1;
        }

        /// <summary>
        /// Gets or sets NValue.
        /// </summary>
        [DataMember(Name = "NValue")]
        public string NValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSubCategorySearch.
        /// </summary>
        [DataMember(Name = "IsSubCategorySearch")]
        public bool IsSubCategorySearch { get; set; }

        /// <summary>
        /// Gets or sets SubCategoryId.
        /// </summary>
        [DataMember(Name = "SubCategoryId")]
        public int SubCategoryId { get; set; }

        /// <summary>
        /// Gets or sets BrandId.
        /// </summary>
        [DataMember(Name = "BrandId")]
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets StoreDepaId.
        /// </summary>
        [DataMember(Name = "StoreDepaId")]
        public int StoreDepaId { get; set; }

        /// <summary>
        /// Gets or sets CategoryId.
        /// </summary>
        [DataMember(Name = "CategoryId")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsUPCCodeSearch.
        /// </summary>
        [DataMember(Name = "IsUPCCodeSearch")]
        public bool IsUPCCodeSearch { get; set; }

        /// <summary>
        /// Gets or sets NodeId.
        /// </summary>
        [DataMember(Name = "NodeId")]
        public int NodeId { get; set; }
    }
}
