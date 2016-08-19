using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public class AdditionalItem_View
    {
        public AdditionalItem_View()
        {
            this.ItemMarketGroup = new List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>();
        }
        #region 商品基本資訊
        /// <summary>
        /// Item Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Item Category
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// Item品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 原價
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 目前商品可銷售數量
        /// </summary>
        public int SellingQty { get; set; }

        /// <summary>
        /// 商品規格
        /// </summary>
        public List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>  ItemMarketGroup { get; set; }

        /// <summary>
        /// 商品狀態
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 是否隱藏
        /// </summary>
        public int ShowOrder { get; set; }

        // Item 圖片路徑
        public string ImagePath { get; set; }
        #endregion

    }
}
