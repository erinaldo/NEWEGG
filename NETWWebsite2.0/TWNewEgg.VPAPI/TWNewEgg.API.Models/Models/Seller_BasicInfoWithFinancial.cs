using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class Seller_BasicInfoWithFinancial
    {
        #region BasicInfo
        public int SellerID { get; set; }
        public string SellerEmail { get; set; }
        public string SellerCountryCode { get; set; }
        public string LanguageCode { get; set; }
        public string FTP { get; set; }
        public string StoreMenu { get; set; }
        public string StoreWebSite { get; set; }
        public string SellerName { get; set; }
        public string AccountTypeCode { get; set; }
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
        public string CountryCode { get; set; }
        public Nullable<System.DateTime> ActiveatedDate { get; set; }
        public Nullable<int> ActiveatedUserID { get; set; }
        public string SellerShortName { get; set; }
        public string ComSellerAdd { get; set; }
        public string ComCity { get; set; }
        public string ComSellerState { get; set; }
        public string ComZipcode { get; set; }
        public string ComCountryCode { get; set; }
        public string Currency { get; set; }

        #endregion

        #region Financial Info

        public string SWIFTCode { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string BankBranchName { get; set; }
        public string BankBranchCode { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAddress { get; set; }
        public string BankCity { get; set; }
        public string BankState { get; set; }
        public string BankCountryCode { get; set; }
        public string BankZipCode { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryAddress { get; set; }
        public string BeneficiaryCity { get; set; }
        public string BeneficiaryState { get; set; }
        public string BeneficiaryCountryCode { get; set; }
        public string BeneficiaryZipcode { get; set; }

        #endregion
    }
}
