using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the EnumServiceType class.
    /// </summary>
    [DataContract]
    public enum EnumServiceType
    {
        /// <summary>
        /// Gets or sets ExtendedWarranty.
        /// </summary>
        [EnumMember]
        ExtendedWarranty,

        /// <summary>
        /// Gets or sets DataRecovery.
        /// </summary>
        [EnumMember]
        DataRecovery
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [DataContract]
    public class UIShoppingItemServiceInfo
    {
        /// <summary>
        /// SNET Prefix.
        /// </summary>
        public const string SNETPrefix = "SNET";

        /// <summary>
        /// Data Recovery Prefix.
        /// </summary>
        public const string DataRecoveryPrefix = "DS";

        /// <summary>
        /// Install Net Suffix.
        /// </summary>
        public const string InstallNetSuffix = "IN";

        /// <summary>
        /// Tech Support Suffix.
        /// </summary>
        public const string TechSupportSuffix = "TS";

        /// <summary>
        /// Initializes a new instance of the UIShoppingItemServiceInfo class.
        /// </summary>
        public UIShoppingItemServiceInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UIShoppingItemServiceInfo class.
        /// </summary>
        /// <param name="shoppingItemInfo">Shopping Item Info.</param>
        /// <param name="serviceType">Service type.</param>
        public UIShoppingItemServiceInfo(UIShoppingItemInfo shoppingItemInfo, EnumServiceType serviceType)
        {
            if (shoppingItemInfo != null)
            {
                this.ItemNumber = shoppingItemInfo.ItemNumber;
                this.Title = shoppingItemInfo.Title;
                this.ImageUrl = shoppingItemInfo.ImageUrl;
                this.ServiceItemInfoList = new List<UIServiceItemInfo>();
                this.ServiceType = serviceType;

                this.ServiceItemInfoList.Add(new UIServiceItemInfo
                {
                    ItemNumber = this.GetServiceItemSuffix(serviceType),
                    Description = Globalization.ShoppingCart.None
                });

                switch (serviceType)
                {
                    case EnumServiceType.ExtendedWarranty:
                        this.ServiceItemInfoList.AddRange(this.GetServiceItemList(shoppingItemInfo.WarrantyGroupList));
                        break;
                    case EnumServiceType.DataRecovery:
                        this.ServiceItemInfoList.AddRange(this.GetServiceItemList(shoppingItemInfo.DriveSaverList));
                        break;
                    default:
                        break;
                }

                var serviceItemNumbers = (from serviceItemInfo in this.ServiceItemInfoList
                                                  where serviceItemInfo.IsSelected
                                                  select serviceItemInfo.ItemNumber).ToList();

                this.ServiceItemNumberSelected = serviceItemNumbers.Count > 0
                                                     ? serviceItemNumbers.First()
                                                     : this.GetServiceItemSuffix(serviceType);
            }
        }

        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [DataMember(Name = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets ImageUrl.
        /// </summary>
        [DataMember(Name = "ImageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets ServiceItemInfoList.
        /// </summary>
        [DataMember(Name = "ServiceItemInfoList")]
        public List<UIServiceItemInfo> ServiceItemInfoList { get; set; }

        /// <summary>
        /// Gets or sets ServiceItemNumberSelected.
        /// </summary>
        [DataMember(Name = "ServiceItemNumberSelected")]
        public string ServiceItemNumberSelected { get; set; }

        /// <summary>
        /// Gets or sets ServiceType.
        /// </summary>
        [DataMember(Name = "ServiceType")]
        public EnumServiceType ServiceType { get; set; }

        /// <summary>
        /// Convert UIWarrantyGroupInfos to UIServiceItemInfos.
        /// </summary>
        /// <param name="groupInfos">Service group infos.</param>
        /// <returns>Service item infos.</returns>
        private IEnumerable<UIServiceItemInfo> GetServiceItemList(ICollection<UIWarrantyGroupInfo> groupInfos)
        {
            var serviceItemInfos = new List<UIServiceItemInfo>();

            if (groupInfos != null && groupInfos.Count > 0)
            {
                foreach (var warrantyGroupInfo in
                    groupInfos.Where(warrantyGroupInfo => warrantyGroupInfo != null 
                                                        && warrantyGroupInfo.WarrantyItemList != null 
                                                        && warrantyGroupInfo.WarrantyItemList.Count > 0))
                {
                    var innerGroupInfo = warrantyGroupInfo;
                    serviceItemInfos.AddRange(
                        innerGroupInfo.WarrantyItemList.Select(
                            warrantyItemInfo => new UIServiceItemInfo
                                                    {
                                                        ItemNumber = warrantyItemInfo.ItemNumber,
                                                        Description = string.Format(Globalization.ShoppingCart.SNETDescription, warrantyItemInfo.Year, innerGroupInfo.GroupName), 
                                                        IsSelected = warrantyItemInfo.IsSelect,
                                                        UnitPrice = warrantyItemInfo.UnitPrice
                                                    }));
                }
            }

            return serviceItemInfos;
        }

        /// <summary>
        /// Get the suffix of the service item.
        /// </summary>
        /// <param name="serviceType">Service type.</param>
        /// <returns>Suffix of service type.</returns>
        private string GetServiceItemSuffix(EnumServiceType serviceType)
        {
            switch (serviceType)
            {
                case EnumServiceType.ExtendedWarranty:
                    return SNETPrefix;
                case EnumServiceType.DataRecovery:
                    return DataRecoveryPrefix;
                default:
                    return string.Empty;
            }
        }
    }
}
