using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSELLERPORTALDB.Models
{
    [Table("Seller_BasicInfo")]
    public class Seller_BasicInfo
    {
        // Memo: Seller_BasicInfo_log.cs 因為目前用不到, 先從專案exclude掉, 確認沒問題後擇期刪除  -- Ron

        // Seller類別
        public enum IdentyType
        {
            國內廠商 = 1,
            國外廠商 = 2,
            個人戶 = 3
        }

        // Seller結帳週期類別
        public enum BillingCycleType
        {
            半月結 = 1,
            月結 = 2
        }

        public int SellerID { get; set; }

        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string SellerEmail { get; set; }
        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string AccountTypeCode { get; set; }
        public string SellerCountryCode { get; set; }
        public string LanguageCode { get; set; }
        public string FTP { get; set; }
        public string StoreMenu { get; set; }
        public string StoreWebSite { get; set; }
        public string SellerName { get; set; }
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
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> ActiveatedDate { get; set; }
        public Nullable<int> ActiveatedUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> InUserID { get; set; }
        public string SellerShortName { get; set; }
        public string ComSellerAdd { get; set; }
        public string ComCity { get; set; }
        public string ComSellerState { get; set; }
        public string ComZipcode { get; set; }
        public string ComCountryCode { get; set; }
        public string Currency { get; set; }

        /// <summary>
        /// 1：國內廠商
        /// 2：國外廠商
        /// 3：個人戶
        /// </summary>
        public Nullable<int> Identy { get; set; }
        /// <summary>
        /// 1：半月結
        /// 2：月結
        /// </summary>
        public Nullable<int> BillingCycle { get; set; }
    }
}
