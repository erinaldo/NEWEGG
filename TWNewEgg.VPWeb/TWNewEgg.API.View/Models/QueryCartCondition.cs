using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class QueryCartCondition
    {
        public enum QuerySO
        {
            訂單編號 = 0,
            收據編號 = 1,
            客戶名稱 = 2,
            商家商品編號 = 3,
            新蛋商品編號 = 4,
            客戶電話 = 5,
            商品標題 = 6,
            生產廠商 = 7
        }
    }
}
