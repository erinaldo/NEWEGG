using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// List for request's parameters' name.
    /// </summary>
    public class RequestParameterName
    {
        /// <summary>
        /// Item List for AddToCart Action.
        /// </summary>
        public const string ItemList = "itemlist";

        /// <summary>
        /// SNET, DS list for AddToCart Action.
        /// </summary>
        public const string EwtList = "ewtlist";

        /// <summary>
        /// Item number for remove item from shopping cart.
        /// Item number for add item to wish list.
        /// </summary>
        public const string ItemNumber = "itemNumber";

        /// <summary>
        /// Query string paramenter for sent number.
        /// </summary>
        public const string SnetNumber = "snetNumber";

        /// <summary>
        /// Item number for remove item from shopping cart.
        /// </summary>
        public const string ItemQty = "itemQty";

        /// <summary>
        /// True or false value whether action is add to wish list.
        /// </summary>
        public const string IsAddToWishList = "isAddToWishList";

        /// <summary>
        /// True or false value whether add to wish list is success.
        /// </summary>
        public const string IsAddToWishListSuccess = "isAddToWishListSuccess";

        /// <summary>
        /// Warranty group model.
        /// </summary>
        public const string WarrantyGroupModel = "warrantyGroupModel";

        /// <summary>
        /// Snet group model.
        /// </summary>
        public const string SnetGroupModel = "snetGroupModel";

        /// <summary>
        /// Item image url.
        /// </summary>
        public const string ItemImagrUrl = "itemImageUrl";

        /// <summary>
        /// Item title string.
        /// </summary>
        public const string ItemTitle = "itemTitle";

        /// <summary>
        /// Item service info string.
        /// </summary>
        public const string ItemServieInfo = "itemServiceInfo";

        /// <summary>
        /// Main item number.
        /// </summary>
        public const string MainItemNumber = "mainItemNumber";

        /// <summary>
        /// Promo code infos string.
        /// </summary>
        public const string PromoCodeInfos = "promoCodeInfos";

        /// <summary>
        /// Promo codes.
        /// </summary>
        public const string PromoCodes = "promoCodes";

        /// <summary>
        /// Promo codes deleted.
        /// </summary>
        public const string PromoCodesDeleted = "promoCodesDeleted";

        /// <summary>
        /// SO Number.
        /// </summary>
        public const string SONumber = "soNumber";

        /// <summary>
        /// Reason Code.
        /// </summary>
        public const string ReasonCode = "reasonCode";

        /// <summary>
        /// Reason Description.
        /// </summary>
        public const string ReasonDescription = "reasonDescription";

        /// <summary>
        /// Order Date for Query.
        /// </summary>
        public const string OrderDate = "orderDate";

        /// <summary>
        /// Sale Order Status.
        /// </summary>
        public const string SOStatus = "soStatus";

        /// <summary>
        /// Tracking Numbers.
        /// </summary>
        public const string TrackingNumbers = "trackingNumbers";

        /// <summary>
        /// Invoice Number.
        /// </summary>
        public const string InvoiceNumber = "invoiceNumber";

        /// <summary>
        /// Pre-SO Number.
        /// </summary>
        public const string PreSONumber = "preSONumber";

        /// <summary>
        /// Is Correlative Combo SO.
        /// </summary>
        public const string IsCorrelativeComboSO = "isCorrelativeComboSO";

        /// <summary>
        /// Combo Id.
        /// </summary>
        public const string ComboId = "comboId";

        /// <summary>
        /// Combo Type.
        /// </summary>
        public const string ComboType = "comboType";

        /// <summary>
        /// Description (category name).
        /// </summary>
        public const string Description = "description";

        /// <summary>
        /// Category Type.
        /// </summary>
        public const string CategoryType = "categoryType";

        /// <summary>
        /// Category Id.
        /// </summary>
        public const string CategoryID = "categoryId";

        /// <summary>
        /// Store Id.
        /// </summary>
        public const string StoreID = "storeId";

        /// <summary>
        /// Store Type.
        /// </summary>
        public const string StoreType = "storeType";

        /// <summary>
        /// Store Depa Id.
        /// </summary>
        public const string StoreDepaId = "storeDepaId";

        /// <summary>
        /// Page Number.
        /// </summary>
        public const string PageNumber = "page";

        /// <summary>
        /// Node Id.
        /// </summary>
        public const string NodeId = "nodeId";

        /// <summary>
        /// Show See All Deals.
        /// </summary>
        public const string ShowSeeAllDeals = "ssad";

        /// <summary>
        /// Is Subcategory URL.
        /// </summary>
        public const string IsSubCategory = "isSubCategory";

        /// <summary>
        /// Is Special Store.
        /// </summary>
        public const string IsSpecialStore = "isSpecialStore";

        /// <summary>
        /// N Value.
        /// </summary>
        public const string NValue = "NValue";

        /// <summary>
        /// Title Parameter Name.
        /// </summary>
        public const string Title = "title";

        /// <summary>
        /// Depa Parameter Name.
        /// </summary>
        public const string Depa = "depa";

        /// <summary>
        /// Parent Category Id.
        /// </summary>
        public const string ParentCategoryId = "parentCategoryId";

        /// <summary>
        /// Address Id.
        /// </summary>
        public const string AddressId = "AddressId";

        /// <summary>
        /// Transaction Number.
        /// </summary>
        public const string TransactionNumber = "transactionNumber";
    }
}
