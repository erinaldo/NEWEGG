using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class SellerCreationResult
    {
        /// <summary>
        /// SellerID
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 使用者ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 新使用者亂數
        /// </summary>
        public string RanCode { get; set; }
    }
}
