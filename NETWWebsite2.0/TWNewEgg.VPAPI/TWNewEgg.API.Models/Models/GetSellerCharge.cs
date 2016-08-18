using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class GetSellerCharge
    {
        /// <summary>
        /// SellerID  (nullable)
        /// </summary>
        public int? SellerID { set; get; }

        /// <summary>
        /// 商家區域
        /// </summary>
        public string CountryCode { set; get; }

        /// <summary>
        /// 商家收費種類
        /// </summary>
        public string ChargeType { set; get; }

        /// <summary>
        /// 類別代號
        /// </summary>
        public int? CategoryID { set; get; }

    }
}
