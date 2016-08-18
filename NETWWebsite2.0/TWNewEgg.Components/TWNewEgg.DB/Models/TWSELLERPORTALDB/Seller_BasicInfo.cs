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
        // Memo: Seller_BasicInfo_log.cs �]���ثe�Τ���, ���q�M��exclude��, �T�{�S���D��ܴ��R��  -- Ron

        // Seller���O
        public enum IdentyType
        {
            �ꤺ�t�� = 1,
            ��~�t�� = 2,
            �ӤH�� = 3
        }

        // Seller���b�g�����O
        public enum BillingCycleType
        {
            �b�뵲 = 1,
            �뵲 = 2
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
        /// 1�G�ꤺ�t��
        /// 2�G��~�t��
        /// 3�G�ӤH��
        /// </summary>
        public Nullable<int> Identy { get; set; }
        /// <summary>
        /// 1�G�b�뵲
        /// 2�G�뵲
        /// </summary>
        public Nullable<int> BillingCycle { get; set; }
    }
}
