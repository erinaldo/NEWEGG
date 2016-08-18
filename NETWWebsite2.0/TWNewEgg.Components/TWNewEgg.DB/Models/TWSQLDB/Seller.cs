using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("seller")]
    public class Seller
    {
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

        public Seller()
        {
            CreateDate = DateTime.UtcNow.AddHours(8);
        }
        /// <summary>
        /// 商家代碼
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        /// <summary>
        /// 商家名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商家全名
        /// </summary>
        public string EngName { get; set; }
        /// <summary>
        /// 商家描述
        /// </summary>
        public string Description { get; set; }
        public Nullable<int> CountryID { get; set; }
        public string CurrencyType { get; set; }
        public string TableName { get; set; }
        /// <summary>
        /// 建立者代碼
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public Nullable<System.DateTime> UpdateDate { get; set; }
        /// <summary>
        /// 帳戶類型
        /// </summary>
        public string AccountType { get; set; }
        /// <summary>
        /// 統編
        /// </summary>
        public string CompanyCode { get; set; }
        public string ACCT_GROUP { get; set; }
        public string PUR_ORG { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Region { get; set; }
        public string Sortl { get; set; }
        public string Address { get; set; }
        public string TELF1 { get; set; }
        public string TELF2 { get; set; }
        public string TELFX { get; set; }
        public string LANGUAGE { get; set; }
        public string VAT_NO { get; set; }
        public string ZTERM { get; set; }
        public string RECON_ACCT { get; set; }
        public string ACTION_CODE { get; set; }
        /// <summary>
        /// 0：Seller update from SellerPorta
        /// 1：Seller update to sap
        /// </summary>
        public int? Istosap { get; set; }
        public string AboutInfo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string State { get; set; }
        public string ComAdd { get; set; }
        /// <summary>
        /// A- Acitve
        /// I-InActive
        /// C-Close
        /// </summary>
        public string Status { get; set; }
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