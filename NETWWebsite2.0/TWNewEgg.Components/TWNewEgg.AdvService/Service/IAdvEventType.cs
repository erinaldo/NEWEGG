using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.SearchService.Service;
using TWNewEgg.AdvService.Models;
using TWNewEgg.ItemService.Service;

namespace TWNewEgg.AdvService.Service
{
    public interface IAdvEventType
    {
        /// <summary>
        /// 新增AdvEventType
        /// </summary>
        /// <param name="arg_oAdvEvent">新增的AdvEvent</param>
        /// <returns>新增AdvEvent的ID; 0:新增失敗</returns>
        int AddAdvEventType(AdvEventType arg_oAdvEventType);

        /// <summary>
        /// 取得所有的AdvEventType列表
        /// </summary>
        /// <returns>null或是AdvEventType的列表</returns>
        List<AdvEventType> GetAdvEventTypeList();

        /// <summary>
        /// 查詢Name含有關鍵字的AdvEventType
        /// </summary>
        /// <param name="arg_strKeyword">關鍵字</param>
        /// <returns>null或是AdvEventType的列表</returns>
        List<AdvEventType> GetAdvEventTypeListByName(string arg_strKeyword);

        /// <summary>
        /// 查詢CSS含有關鍵字的AdvEventType
        /// </summary>
        /// <param name="arg_strKeyword">關鍵字</param>
        /// <returns>null或是AdvEventType的列表</returns>
        List<AdvEventType> GetAdvEventTypeListByCSS(string arg_strCSS);

        /// <summary>
        /// 根據ID取得AdvEventType物件
        /// </summary>
        /// <param name="arg_nAdvEventType">ID</param>
        /// <returns>null或AdvEventType物件</returns>
        AdvEventType GetAdvEventTypeById(int arg_nAdvEventType);

        /// <summary>
        /// 修改AdvEventType
        /// </summary>
        /// <param name="arg_oAdvEventType">修改的AdvEventType物件</param>
        /// <returns>true:修改成功, false:修改失敗</returns>
        bool UpdateAdvEventType(AdvEventType arg_oAdvEventType);

        /// <summary>
        /// 根據Country Code及AdvType取得有效期間內的AdvEventType的列表
        /// </summary>
        /// <param name="arg_numAdvType">AdvType</param>
        /// <returns>AdvEventType列表</returns>
        List<AdvEventType> GetActiveAdvEventTypeListByAdvType(int arg_numAdvType);
    }
}
