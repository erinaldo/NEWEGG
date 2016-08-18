using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Vendor
{
    public class Marketplace_View
    {
        /// <summary>
        /// 公司名稱
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string CompanyUnifiedNumber { get; set; }
        /// <summary>
        /// 公司電話
        /// </summary>
        public string CompanyPhone { get; set; }
        /// <summary>
        /// 公司網址
        /// </summary>
        public string CompanyOfficialSiteUrl { get; set; }
        /// <summary>
        /// 聯絡人
        /// </summary>
        public string CompanyContact { get; set; }
        /// <summary>
        /// 聯絡電話
        /// </summary>
        public string CompanyContactPhone { get; set; }
        /// <summary>
        /// 聯絡EMAIL
        /// </summary>
        public string CompanyContactEmail { get; set; }
        /// <summary>
        /// 希望上架類別
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 其它說明
        /// </summary>
        public string Remark { get; set; }
    }
}
