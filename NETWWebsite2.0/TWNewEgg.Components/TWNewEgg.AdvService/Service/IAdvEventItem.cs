using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.AdvService.Models;
using TWNewEgg.ItemService.Service;

namespace TWNewEgg.AdvService.Service
{
    public interface IAdvEventItem
    {
        /// <summary>
        /// 轉換AdvEvent為AdvEventDisplay
        /// </summary>
        /// <param name="argITem">Item</param>
        /// <param name="argAdvEvent">AdvEvent</param>
        /// <param name="fromAdvEventDisplay">欲轉換的AdvEventDisplay</param>
        /// <returns>轉換完成的AdvEventDisplay</returns>
        AdvEventDisplay TransItem2AdvEventDisplay(Item argITem, AdvEvent argAdvEvent, AdvEventDisplay fromAdvEventDisplay);

        /// <summary>
        /// 根據傳入的AdvEventTypeID, 回傳旗下的廣告, 並且旗下的廣告資料皆已轉換完成
        /// </summary>
        /// <param name="arg_numAdvEventTypeId">AdvEventTypeID</param>
        /// <returns>AdvEventDisplay列表或是null</returns>
        List<AdvEventDisplay> GetAdvEventDisplayListByAdvEventTypeId(int arg_numAdvEventTypeId);

        /// <summary>
        /// 根據傳入的AdvEventTypeID, 回傳旗下Active的廣告, 並且旗下的廣告資料皆已轉換完成
        /// </summary>
        /// <param name="arg_numAdvEventTypeId">AdvEventTtypeId</param>
        /// <returns>AdvEventDisplay列表或是null</returns>
        List<AdvEventDisplay> GetActiveAdvEventDisplayListByAdvEventTypeId(int arg_numAdvEventTypeId);
    }
}
