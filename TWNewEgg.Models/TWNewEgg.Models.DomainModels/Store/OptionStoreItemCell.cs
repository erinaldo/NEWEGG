using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// 任選館的單一商品, 比Store的商品多了一些屬性.
    /// </summary>
    public class OptionStoreItemCell : StoreItemCell
    {
        /// <summary>
        /// 規格資訊
        /// </summary>
        public List<FormatInfo> FormatList { get; set; }

        /// <summary>
        /// 商品尚未折扣前的金額
        /// </summary>
        public decimal OriginalPrice { get; set; }
        
        /// <summary>
        /// 單次可購買的數量限制.
        /// </summary>
        public int SellingQty { get; set; }
        
        /// <summary>
        /// 是否售完.
        /// </summary>
        public bool IsOutOfStock { get; set; }

        /// <summary>
        /// 是否已選購.
        /// </summary>
        public bool IsChoose { get; set; }

        /// <summary>
        /// 已經選取的數量
        /// </summary>
        public int PickQty { get; set; }
    }
}
