using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class SellerCreation
    {
        /// <summary>
        /// 商家電子郵件  (not null)
        /// </summary>      
        public string SellerEmail { set; get; }

        /// <summary>
        /// 商家區域
        /// </summary>
        public string Region { set; get; }

        /// <summary>
        /// 商家語言
        /// </summary>
        public string Language { set; get; }

        /// <summary>
        /// Account Initial Status
        /// A = Active
        /// I = Inactive
        /// C = Close
        /// </summary>
        public string Status { set; get; }

        /// <summary>
        /// AccountTypeCode (not null)
        /// S = Seller
        /// D = Daren
        /// V = Vendor
        /// </summary>
        public string AccountType { set; get; }

        /// <summary>
        /// 建立者UserID  (not null)
        /// </summary>
        public int InUserID { get; set; }

        /// <summary>
        /// User群組（建立User用）  (not null)
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// 幣別 (not null)
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 統一編號/身分證 (not null)
        /// </summary>
        public string CompanyCode { set; get; }

        /// <summary>
        /// 商家名稱 (not null)
        /// </summary>
        public string SellerName { get; set; }

        /// <summary>
        /// 廠商身分別(1. 國內廠商 2.國外廠商 3.個人戶)
        /// </summary>
        public Nullable<int> Identy { get; set; }

        /// <summary>
        /// 付款方式(1.半月結 2.月結)
        /// </summary>
        public Nullable<int> BillingCycle { get; set; }

        /// <summary>
        /// 公司統一編號或個人身分證
        /// </summary>
        public string CompanyTaxId_Identity { get; set; }
    }
}
