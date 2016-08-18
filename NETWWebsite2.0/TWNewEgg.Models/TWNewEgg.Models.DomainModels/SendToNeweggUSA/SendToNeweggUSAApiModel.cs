using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TWNewEgg.Models.DomainModels.SendToNeweggUSA
{
   public class SendToNeweggUSAApiModel
    {

        public string itemNumber{get;set;}
        public float price{get;set;}
        public decimal shippingCharge{get;set;}
        public int quantity{get;set;}
        public string WarehouseNumber{get;set;} 
        public string refPONumber{get;set;} 
        public  CustomerInfo customerBillingInfo{get;set;} 
        public CustomerInfo customerShippingInfo{get;set;} 
        public string shipViaCode{get;set;}
        public bool isControlled{get;set;} 
        public  string commissionNote {get;set;} 
        public   string customerOwnShippingAccount  {get;set;} 
        public string specialComment  {get;set;}
        public bool isNeweggFlash { get; set; }
    
        public class CustomerInfo
    {
        /// <summary>
        /// ContactWith
        /// </summary>
        public string ContactWith;

        /// <summary>
        /// Address1
        /// </summary>
        public string Address1;

        /// <summary>
        /// Address2
        /// </summary>
        public string Address2;


        /// <summary>
        /// State
        /// </summary>
        public string State;

        /// <summary>
        /// City
        /// </summary>
        public string City;

        /// <summary>
        /// Country
        /// </summary>
        public string Country;

        /// <summary>
        /// ZipCode
        /// </summary>
        public string ZipCode;

        /// <summary>
        /// CompanyName
        /// </summary>
        public string CompanyName;

        /// <summary>
        /// HomePhone
        /// </summary>
        public string HomePhone;

        /// <summary>
        /// Fax
        /// </summary>
        public string Fax;

    }
        
        
   }
}
