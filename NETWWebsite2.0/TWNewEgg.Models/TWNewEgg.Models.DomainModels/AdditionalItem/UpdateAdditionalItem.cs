using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.AdditionalItem
{
    /// <summary>
    /// 更新加價購商品
    /// </summary>
    public class UpdateAdditionalItem
    {
        /// <summary>
        /// 加價購商品清單
        /// </summary>
        public List<AdditionalItem> AdditionalItemCell { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateUser { get; set; }

        public UpdateAdditionalItem()
        {
            AdditionalItemCell = new List<AdditionalItem>();
        }
    }

    /// <summary>
    /// 加價購商品
    /// </summary>
    public class AdditionalItem
    {
        /// <summary>
        /// 加價購 ID
        /// </summary>
        public int AdditionalNum { get; set; }

        /// <summary>
        /// 商品排序
        /// </summary>
        public int Sequence { get; set; }
    }
}
