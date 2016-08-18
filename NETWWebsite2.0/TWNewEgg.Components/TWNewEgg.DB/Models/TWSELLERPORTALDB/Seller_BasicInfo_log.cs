using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSELLERPORTALDB.Models
{
    [Table("Seller_BasicInfo_log")]
    public class Seller_BasicInfo_log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SellerID { get; set; }
        public string SellerEmail { get; set; }
        public string Authority { get; set; }
        public int SellerCountryCode { get; set; }
        public Nullable<int> LanguageCode { get; set; }
        public string FTP { get; set; }
        public string StoreMenu { get; set; }
        public string StoreWebSite { get; set; }
        public string SellerName { get; set; }
        public string AccountTypeCode { get; set; }
        public string CSPhoneRegion { get; set; }
        public string CSPhone { get; set; }
        public string CSPhoneExt { get; set; }
        public string CSEmailAddress { get; set; }
        public string SellerStatus { get; set; }
        public string SellerLogoURL { get; set; }
        public string AboutInfo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneRegion { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }
        public string EmailAddress { get; set; }
        public string SellerAddress { get; set; }
        public string City { get; set; }
        public string SellerState { get; set; }
        public string Zipcode { get; set; }
        public string CompanyCode { get; set; }
        public Nullable<int> CountryCode { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> ActiveatedDate { get; set; }
        public Nullable<int> ActiveatedUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> InUserID { get; set; }
    }
}
