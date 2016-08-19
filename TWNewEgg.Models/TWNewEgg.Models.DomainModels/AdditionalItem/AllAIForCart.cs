using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Item;

namespace TWNewEgg.Models.DomainModels.AdditionalItem
{
    public class AllAIForCart
    {
        public enum Issuccess
        {
            OK=10000,
            CANTFINDITEM = 10001,
            WRONGCARTTYPE = 10002,
            WRONGDELIVTYPE = 10003
        }
        public ItemDetail itemDetail { get; set; }
        public List<ItemMarketGroup> itemGroup { get; set; }
        public AIForCartDM additionalItemForCart { get; set; }

        /// <summary>
        /// 商品排序
        /// </summary>
        public int Sequence { get; set; }
    }
}
