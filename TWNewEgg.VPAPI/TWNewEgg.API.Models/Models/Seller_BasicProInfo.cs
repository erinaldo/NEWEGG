using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class Seller_BasicProInfo
    {
        public int SellerID { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }
        public string PhoneRegion { get; set; }
        
        public string CompanyCode { get; set; }
        public string CountryCode { get; set; }
        
        public string SellerName { get; set; }
        //public string CSEmailAddress { get; set; }
        //public string CSPhone { get; set; }
        //public string CSPhoneExt { get; set; }
        //public string CSPhoneRegion { get; set; }
        public string EmailAddress { get; set; }
        public string SellerAddress { get; set; }
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Zipcode { get; set; }
        public string SellerState { get; set; }
        
        public DateTime? CreateDate { get; set; }
        public DateTime? ActiveatedDate { get; set; }
        public int? ActiveatedUserID { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateUserID { get; set; }
        public DateTime? InDate { get; set; }
        public int? InUserID { get; set; }

        public string SellerShortName { get; set; }
        public string ComSellerAdd { get; set; }
        public string ComCity { get; set; }
        public string ComSellerState { get; set; }
        public string ComZipcode { get; set; }
        public string ComCountryCode { get; set; }
        //身分別
        public string Identy { get; set; }
        
        //public string AccountTypeCode { get; set; }
        //public string Authority { get; set; }
        //public string CompanyCode { get; set; }
        
        //public string FTP { get; set; }
        //public int? LanguageCode { get; set; }
        //public string SellerEmail { get; set; }
        //public int SellerCountryCode { get; set; }

        //public string AboutInfo { get; set; }
        //public string SellerLogoURL { get; set; }
        
        
        //public string SellerStatus { get; set; }
        //public string StoreMenu { get; set; }
        //public string StoreWebSite { get; set; }
        
        
        

    }
}
