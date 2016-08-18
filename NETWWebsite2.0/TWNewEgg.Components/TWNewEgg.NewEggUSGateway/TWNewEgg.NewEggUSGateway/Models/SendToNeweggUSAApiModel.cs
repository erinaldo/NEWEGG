using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.NewEggUSGateway.Models
{
    public class SendToNeweggUSAApiModel
    {
   

        public string commissionNote { get; set; }
        public SendToNeweggUSAApiModel.CustomerInfo customerBillingInfo { get; set; }
        public string customerOwnShippingAccount { get; set; }
        public SendToNeweggUSAApiModel.CustomerInfo customerShippingInfo { get; set; }
        public bool isControlled { get; set; }
        public bool isNeweggFlash { get; set; }
        public string itemNumber { get; set; }
        public float price { get; set; }
        public int quantity { get; set; }
        public string refPONumber { get; set; }
        public decimal shippingCharge { get; set; }
        public string shipViaCode { get; set; }
        public string specialComment { get; set; }
        public string WarehouseNumber { get; set; }

        public class CustomerInfo
        {
            public string Address1;
            public string Address2;
            public string City;
            public string CompanyName;
            public string ContactWith;
            public string Country;
            public string Fax;
            public string HomePhone;
            public string State;
            public string ZipCode;

        
        }
    }
}
