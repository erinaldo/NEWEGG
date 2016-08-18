using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class GetSellerChargeResult
    {
        /// <summary>
        /// Seller ID
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// Country Code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// 收費種類
        /// </summary>
        public string ChargeType { get; set; }

        /// <summary>
        /// 主類別ID
        /// </summary>
        public int CategoryID;

        /// <summary>
        /// 主類別英文名稱
        /// </summary>
        public string CategoryTitle { get; set; }

        /// <summary>
        /// 主類別中文名稱
        /// </summary>
        public string CategoryDescription { get; set; }

        /// <summary>
        /// 佣金
        /// </summary>
        public decimal Commission { get; set; }
    }
}
