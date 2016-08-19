using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// 用來存放商品單一規格資訊.
    /// </summary>
    public class FormatInfo
    {
        /// <summary>
        /// 此規格品的ItemID
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// 規格的顯示文字.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 單次可購買的數量限制.
        /// </summary>
        public int SellingQty { get; set; }

        /// <summary>
        /// 是否售完.
        /// </summary>
        public bool IsOutOfStock { get; set; }
    }
}
