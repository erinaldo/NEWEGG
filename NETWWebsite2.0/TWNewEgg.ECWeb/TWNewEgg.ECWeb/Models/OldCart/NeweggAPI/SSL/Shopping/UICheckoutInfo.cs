using System.Collections.Generic;
using System.Runtime.Serialization;
using Newegg.Mobile.MvcApplication.Globalization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICheckoutInfo class.
    /// </summary>
    [DataContract]
    public class UICheckoutInfo
    {
        /// <summary>
        /// Initializes a new instance of the UICheckoutInfo class.
        /// </summary>
        public UICheckoutInfo()
        {
            this.Orders = new List<UIOrderInfo>();
            this.DiscountRows = new List<UICheckoutDiscountRowInfo>();
            this.NeedRemovedItems = new List<string>();
        }

        /// <summary>
        /// Gets or sets Orders.
        /// </summary>
        [DataMember(Name = "Orders")]
        public List<UIOrderInfo> Orders { get; set; }

        /// <summary>
        /// Gets or sets DefaultShippingAddress.
        /// </summary>
        [DataMember(Name = "DefaultShippingAddress")]
        public UIAddressInfo DefaultShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets DefaultBillingAddress.
        /// </summary>
        [DataMember(Name = "DefaultBillingAddress")]
        public UIAddressInfo DefaultBillingAddress { get; set; }

        /// <summary>
        /// Gets or sets DefaultCreditCard.
        /// </summary>
        [DataMember(Name = "DefaultCreditCard")]
        public UICreditCardPaymentInfo DefaultCreditCard { get; set; }

        /// <summary>
        /// Gets or sets CheckoutDisplayFeeInfo.
        /// </summary>
        [DataMember(Name = "CheckoutDisplayFeeInfo")]
        public UICheckoutDisplayFeeInfo CheckoutDisplayFeeInfo { get; set; }

        /// <summary>
        /// Gets or sets DiscountRows.
        /// </summary>
        [DataMember(Name = "DiscountRows")]
        public List<UICheckoutDiscountRowInfo> DiscountRows { get; set; }

        /// <summary>
        /// Gets or sets SessionID.
        /// </summary>
        [DataMember(Name = "SessionID")]
        public string SessionID { get; set; }

        /// <summary>
        /// Gets or sets NeedRemovedItems.
        /// </summary>
        [DataMember(Name = "NeedRemovedItems")]
        public List<string> NeedRemovedItems { get; set; }

        /// <summary>
        /// Gets or sets UICoreMetricsInfo.
        /// </summary>
        [DataMember(Name = "UICoreMetricsInfo")]
        public UICoreMetricsInfo UICoreMetricsInfo { get; set; }

        /// <summary>
        /// Gets or sets ShippingPromotionInfo.
        /// </summary>
        [DataMember(Name = "ShippingPromotionInfo")]
        public UIShippingPromotionInfo ShippingPromotionInfo { get; set; }

        /// <summary>
        /// Gets or sets ShoppingUnitList.
        /// </summary>
        [DataMember(Name = "ShoppingUnitList")]
        public List<UIShoppingUnitInfo> ShoppingUnitList { get; set; }

        /// <summary>
        /// Gets or sets ShoppingCartUnitList.
        /// </summary>
        [DataMember(Name = "ShoppingCartUnitList")]
        public List<UIShoppingCartUnitInfo> ShoppingCartUnitList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowPromotionCode.
        /// </summary>
        [DataMember(Name = "IsShowPromotionCode")]
        public bool IsShowPromotionCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CanPlaceOrder.
        /// </summary>
        [DataMember(Name = "CanPlaceOrder")]
        public bool CanPlaceOrder { get; set; }

        /// <summary>
        /// Gets or sets RushOrderFeeInfo.
        /// </summary>
        [DataMember(Name = "RushOrderFeeInfo")]
        public UIRushOrderFeeInfo RushOrderFeeInfo { get; set; }

        /// <summary>
        /// Gets or sets PaypalAmountLimit.
        /// </summary>
        [DataMember(Name = "PaypalAmountLimit")]
        public decimal PaypalAmountLimit { get; set; }

        /// <summary>
        /// Gets or sets Paypal Email Accound.
        /// </summary>
        public string PaypalEmailAccound { get; set; }

        /// <summary>
        /// Gets or sets PaymentInfo.
        /// </summary>
        [DataMember(Name = "PaymentInfo")]
        public UIPaymentInfo PaymentInfo { get; set; }

        /// <summary>
        /// Gets or sets Notes.
        /// </summary>
        [DataMember(Name = "Notes")]
        public List<UINoteItemInfo> Notes { get; set; }

        /// <summary>
        /// Gets or sets UIPreferredAccountPaymentInfo.
        /// </summary>
        [DataMember(Name = "NeweggPreferredAccountInfo")]
        public UIPreferredAccountPaymentInfo NeweggPreferredAccountInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsValidContactWith.
        /// </summary>
        public bool IsValidContactWith
        {
            get
            {
                if (
                    this.DefaultShippingAddress == null ||
                    this.DefaultShippingAddress.Address1 == null ||
                    this.DefaultShippingAddress.Address1.ToLower().IndexOf(Checkout.Delivery) > -1 ||
                    this.DefaultShippingAddress.Address1.ToLower().IndexOf(Checkout.General) > -1 ||
                    (this.DefaultShippingAddress.Address2 != null &&
                        (this.DefaultShippingAddress.Address2.ToLower().IndexOf(Checkout.Delivery) > -1 ||
                            this.DefaultShippingAddress.Address2.ToLower().IndexOf(Checkout.General) > -1)))
                {
                    return false;
                }

                if (
                    this.DefaultBillingAddress == null ||
                    this.DefaultBillingAddress.Address1 == null ||
                    this.DefaultBillingAddress.Address1.ToLower().IndexOf(Checkout.Delivery) > -1 ||
                    this.DefaultBillingAddress.Address1.ToLower().IndexOf(Checkout.General) > -1 ||
                    (this.DefaultBillingAddress.Address2 != null &&
                        (this.DefaultBillingAddress.Address2.ToLower().IndexOf(Checkout.Delivery) > -1 ||
                            this.DefaultBillingAddress.Address2.ToLower().IndexOf(Checkout.General) > -1)))
                {
                    return false;
                }

                return true;
            }

            set 
            {
            }
        }
    }
}
