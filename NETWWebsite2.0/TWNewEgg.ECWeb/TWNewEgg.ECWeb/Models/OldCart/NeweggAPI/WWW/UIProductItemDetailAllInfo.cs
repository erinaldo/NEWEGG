using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductItemDetailAllInfo class.
    /// </summary>
    [DataContract]
    public class UIProductItemDetailAllInfo : UIProductItemDetailInfo
    {
        /// <summary>
        /// Gets or sets uiVolumeDiscountInfo.
        /// </summary>
        [DataMember(Name = "uiVolumeDiscountInfo")]
        public List<UIVolumeDiscountInfo> UIVolumeDiscountInfo { get; set; }

        /// <summary>
        /// Gets or sets uiExtendedWarrantyContent.
        /// </summary>
        [DataMember(Name = "uiExtendedWarrantyContent")]
        public UIExtendedWarrantyContent UIExtendedWarrantyContent { get; set; }

        /// <summary>
        /// Gets or sets DriveSaverGroupList.
        /// </summary>
        [DataMember(Name = "DriveSaverGroupList")]
        public List<UIExtendedWarrantyGroupInfo> DriveSaverGroupList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HasSimilarURL.
        /// </summary>
        [DataMember(Name = "HasSimilarURL")]
        public bool HasSimilarURL { get; set; }

        /// <summary>
        /// Gets or sets NValue.
        /// </summary>
        [DataMember(Name = "NValue")]
        public string NValue { get; set; }
        
        /// <summary>
        /// Gets or sets ItemType.
        /// </summary>
        [DataMember(Name = "ItemType")]
        public UIItemType ItemType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAddToWishList.
        /// </summary>
        [DataMember(Name = "IsAddToWishList")]
        public bool IsAddToWishList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAddToWishListSuccess.
        /// </summary>
        [DataMember(Name = "IsAddToWishListSuccess")]
        public bool IsAddToWishListSuccess { get; set; }

        /// <summary>
        /// Gets or sets AddedToWishListMessage.
        /// </summary>
        [DataMember(Name = "AddedToWishListMessage")]
        public string AddedToWishListMessage { get; set; }

        [DataMember(Name = "Model")]
        public string Model { get; set; }

        [DataMember(Name = "LimitQuantity")]
        public int LimitQuantity { get; set; }

        [DataMember(Name = "PromotionText")]
        public string PromotionText { get; set; }

        [DataMember(Name = "IsHot")]
        public string IsHot { get; set; }

        [DataMember(Name = "SellerId")]
        public string SellerId { get; set; }

        [DataMember(Name = "SellerName")]
        public string SellerName { get; set; }

        [DataMember(Name = "Instock")]
        public bool Instock { get; set; }

        [DataMember(Name = "ReviewSummary")]
        public ReviewSummary ReviewSummary { get; set; }

        [DataMember(Name="BrandInfo")]
        public UIBrandInfo BrandInfo { get; set; }
    }

    public class ReviewSummary
    {
        public string Rating { get; set; }
        public string TotalReviews { get; set; }
    } 
}
