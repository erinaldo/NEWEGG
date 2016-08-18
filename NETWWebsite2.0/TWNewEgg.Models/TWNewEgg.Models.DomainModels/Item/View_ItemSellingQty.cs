using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class View_ItemSellingQty
    {
        /// <summary>
        /// Item.Id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Item.CategoryId
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// Item.Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Item.ShowOrder
        /// </summary>
        public int ShowOrder { get; set; }

        /// <summary>
        /// Product.Id
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Item.Qty
        /// </summary>
        public int ItemQty { get; set; }

        /// <summary>
        /// Item.QtyReg
        /// </summary>
        public int ItemQtyReg { get; set; }

        /// <summary>
        /// Item.SafeQty
        /// </summary>
        public int ItemSafeQty { get; set; }

        /// <summary>
        /// Item.QtyLimit
        /// </summary>
        public int ItemQtyLimit { get; set; }

        /// <summary>
        /// ItemStock.Qty
        /// </summary>
        public int StockQty { get; set; }

        /// <summary>
        /// ItemStock.QtyReg
        /// </summary>
        public int StockQtyReg { get; set; }

        /// <summary>
        /// ItemStock.SafeQty
        /// </summary>
        public int StockSafeQty { get; set; }

        /// <summary>
        /// ItemQty-ItemQtyReg
        /// </summary>
        public int? ItemQtyAmount { get; set; }

        /// <summary>
        /// StockQty-StockQtyReg
        /// </summary>
        public int? StockQtyAmount { get; set; }

        /// <summary>
        /// 賣場實際可銷售數量
        /// </summary>
        public int? SellingQty { get; set; }
    }
}
