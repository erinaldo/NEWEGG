using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    [System.Xml.Serialization.XmlInclude(typeof(OrderInfo9SI))]
    [System.Xml.Serialization.XmlInclude(typeof(OrderInfo))]
    [Serializable]
    public class OrderInfoBase
    {
        /// <summary>
        /// EDI ID
        /// </summary>
        public string EDIID;


        /// <summary>
        /// Customer Number
        /// </summary>
        public int CustomerNumber;

        /// <summary>
        /// SODate 2013-03-09 12:00:00
        /// </summary>
        public string SODate;

        /// <summary>
        /// SOAmount
        /// </summary>
        public float SOAmount;

        /// <summary>
        /// ShipViaCode
        /// </summary>
        public string ShipViaCode;

        /// <summary>
        /// CustomerOwnShippingAccount
        /// </summary>
        public string CustomerOwnShippingAccount;

        /// <summary>
        /// IPAddress
        /// </summary>
        public string IPAddress;


        /// <summary>
        /// CompanyCode
        /// </summary>
        public int CompanyCode;

        /// <summary>
        /// TaxRate
        /// </summary>
        public int TaxRate;

        /// <summary>
        /// CustomerPONumber
        /// </summary>
        public string CustomerPONumber;

        /// <summary>
        /// ReferenceSONumber
        /// </summary>
        public string ReferenceSONumber;

        /// <summary>
        /// PayTermsCode
        /// </summary>
        public string PayTermsCode;

        /// <summary>
        /// SalesPostUser
        /// </summary>
        public string SalesPostUser;

        /// <summary>
        /// SalesPostDate
        /// </summary>
        public string SalesPostDate;

        /// <summary>
        /// AcctPostUser
        /// </summary>
        public string AcctPostUser;

        /// <summary>
        /// AcctPostDate
        /// </summary>
        public string AcctPostDate;

        /// <summary>
        /// SalesPerson
        /// </summary>
        public string SalesPerson;

        /// <summary>
        /// CreditCardCharged
        /// </summary>
        public int CreditCardCharged;

        /// <summary>
        /// CreditCardVerifyMark
        /// </summary>
        public string CreditCardVerifyMark;

        /// <summary>
        /// SOMemo
        /// </summary>
        public string SOMemo;

        /// <summary>
        /// SpecialComment
        /// </summary>
        public string SpecialComment;

        /// <summary>
        /// HoldMark
        /// </summary>
        public int HoldMark;

        /// <summary>
        /// HoldReason
        /// </summary>
        public string HoldReason;

        /// <summary>
        /// IsNF
        /// </summary>
        public bool IsNF;

        /// <summary>
        /// HoldReason
        /// </summary>
        public CustomerInfo CustomerBillingInfo;

        /// <summary>
        /// HoldReason
        /// </summary>
        public CustomerInfo CustomerShippingInfo;

        /// <summary>
        /// HoldReason
        /// </summary>
        public SellerInfo SellerInfo;

        /// <summary>
        /// itemList
        /// </summary>
        public ItemInfo[] ItemList;
    }
}