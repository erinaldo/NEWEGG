using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class Seller_ContactInfoData
    {
        public string Address { get; set; }
        public string City { get; set; }
        public int ContactTypeID { get; set; }
        public string CountryCode { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public DateTime? InDate { get; set; }
        public int? InUserID { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }
        public string PhoneRegion { get; set; }
        public string PrimaryCode { get; set; }
        public int SellerID { get; set; }
        public int SN { get; set; }
        public string State { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateUserID { get; set; }
        public string ZipCode { get; set; }
    }
}
