using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class ItemMarketGroup
    {
        /// <summary>
        /// Group Id
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Item Id
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 主屬性的Property Id
        /// </summary>
        public int MasterPropertyId { get; set; }

        /// <summary>
        /// 主屬性的Property顯示文字
        /// </summary>
        public string MasterPropertyDisplay { get; set; }

        /// <summary>
        /// 副屬性的Property Id
        /// </summary>
        public int SecondPropertyId { get; set; }

        /// <summary>
        /// 副屬性的Property顯示文字
        /// </summary>
        public string SecondPropertyDisplay { get; set; }

        /// <summary>
        /// 主屬性的Value Id
        /// </summary>
        public int MasterPropertyValueId { get; set; }

        /// <summary>
        /// 主屬性的Value顯示值
        /// </summary>
        public string MasterPropertyValueDisplay { get; set; }

        /// <summary>
        /// 副屬性的Value Id
        /// </summary>
        public int SecondPropertyValueId { get; set; }

        /// <summary>
        /// 副屬性的Value 顯示值
        /// </summary>
        public string SecondPropertyValueDisplay { get; set; }

        /// <summary>
        /// 可銷售數量, 為了避免機器人, 有值統一設為10, 否則為0
        /// </summary>
        public int SellingQty { get; set; }

        /// <summary>
        /// 賣場狀態
        /// </summary>
        public int Status { get; set; }
    }
}
