using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class StorageDetailSPSearch
    {   
        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string inputSellerName { get; set; }

        /// <summary>
        /// 廠商ID
        /// </summary>
        public string inputSellerID { get; set; }

        /// <summary>
        /// (新蛋)商品編號
        /// </summary>
        public int? inputNeweggProductID { get; set; }

        /// <summary>
        /// 商家商品編號
        /// </summary>
        public int? inputSellerProductID { get; set; }
    }
}
