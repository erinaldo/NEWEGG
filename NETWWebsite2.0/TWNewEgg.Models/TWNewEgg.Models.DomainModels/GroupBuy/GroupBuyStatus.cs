using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.GroupBuy
{
    public class GroupBuyStatus
    {
        /// <summary>
        /// GroupBuy Status
        /// </summary>
        public enum StatusEnum
        {
            草稿 = 1,
            待審核 = 2,
            退回 = 3,
            待上檔 = 4,
            上檔中 = 5,
            已售完 = 6,
            已下檔 = 7,
            已移除 = 8
        }
    }
}
