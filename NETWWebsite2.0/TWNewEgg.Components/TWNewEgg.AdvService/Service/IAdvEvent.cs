using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.AdvService.Models;
using TWNewEgg.DB.TWSQLDB.Models;

namespace TWNewEgg.AdvService.Service
{
    public interface IAdvEvent
    {
        /// <summary>
        /// Get a AdvEventDisplay by ID.
        /// </summary>
        /// <param name="ID">Adv Event ID</param>
        /// <returns></returns>
        TWNewEgg.DB.TWSQLDB.Models.AdvEvent GetOneAdvEventByID(int ID);

        /// <summary>
        /// Get a AdvEventDisplay by hash code.
        /// </summary>
        /// <param name="hashCode">Adv Event Hash Code</param>
        /// <returns></returns>
        AdvEventDisplay GetOneAdvEventByHashCode(string hashCode);

        /// <summary>
        /// Get AdvEventDisplay  by IDs.
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="orderCondition"></param>
        /// <returns></returns>
        IEnumerable<AdvEventDisplay> GetAdvEventsByIDs(List<int> IDs, int orderCondition);

        /// <summary>
        /// Get AdvEventDisplay  by Hash Codes.
        /// </summary>
        /// <param name="IDs">List of Adv Event Hash Codes</param>
        /// <returns></returns>
        IEnumerable<AdvEventDisplay> GetAdvEventsByHashCodes(List<string> hashCodes, int orderCondition);

        /// <summary>
        /// Get AdvEventDisplay only for opening.
        /// </summary>
        /// <param name="dateTimeNow"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="clearItemLinkifNotStart"></param>
        /// <param name="reallyDateTimeNow"></param>
        /// <returns></returns>
        IEnumerable<AdvEventDisplay> GetEveryDayTimeLimitAdvEvents(int advType, DateTime dateTimeNow, DateTime? startDate, DateTime? endDate, bool clearItemLinkifNotStart, DateTime? reallyDateTimeNow);

        /// <summary>
        /// Get Recommend Items from AdvEvent by AdvEvent's hash code
        /// </summary>
        /// <param name="hashCode"></param>
        /// <returns></returns>
        IEnumerable<AdvEventDisplay> GetRecommendFromAdvEventByHashCode(string hashCode);

        /// <summary>
        /// Get Recommend Items from AdvEvent which is close dateTimeNow
        /// </summary>
        /// <param name="dateTimeNow"></param>
        /// <returns></returns>
        IEnumerable<AdvEventDisplay> GetRecommendFromLimitAdvEventByDateTime(DateTime dateTimeNow);

        /// <summary>
        /// Get Recommend Items by string 
        /// </summary>
        /// <param name="recommendIDs"></param>
        /// <param name="imgSize"></param>
        /// <returns></returns>
        IEnumerable<AdvEventDisplay> GetRecommendFromString(string recommendIDs, int imgSize);
        /// <summary>
        /// Get Recommend Items by string 
        /// </summary>
        /// <param name="extraApi"></param>
        /// <param name="extraMethod"></param>
        /// <param name="extraArg"></param>
        /// <param name="imgSize"></param>
        /// <returns></returns>
        IEnumerable<AdvEventDisplay> GetRecommendFromString(string extraApi, string extraMethod, string extraArg, int extraNumber, int imgSize);

        /// <summary>
        /// Get Recommend Items by string 
        /// </summary>
        /// <param name="recommendIDs"></param>
        /// <param name="extraApis"></param>
        /// <param name="extraMethods"></param>
        /// <param name="extraArgs"></param>
        /// <param name="imgSize"></param>
        /// <returns></returns>
        IEnumerable<AdvEventDisplay> GetRecommendFromString(string recommendIDs, List<string> extraApis, List<string> extraMethods, List<string> extraArgs, int imgSize);

        /// <summary>
        /// 根據AdvEventType取得旗下的所有AdvEvent
        /// </summary>
        /// <param name="arg_nAdvTypeId">AdvEventType.ID</param>
        /// <returns>null or List of AdvEvent</returns>
        List<AdvEvent> GetAllAdvEventByAdvEventTypeId(int arg_nAdvTypeId);

        /// <summary>
        /// 根據AdvEventType取得旗下的所有上線的AdvEvent
        /// </summary>
        /// <param name="arg_nAdvTypeId"></param>
        /// <returns></returns>
        List<AdvEvent> GetActiveAdvEventByAdvEventTypeId(int arg_nAdvTypeId);

        /// <summary>
        /// 修改AdvEvent
        /// </summary>
        /// <param name="arg_oAdvEvent">要修改的AdvEvent</param>
        /// <returns>true:修改成功; false: 修改失敗</returns>
        bool UpdateAdvEvent(AdvEvent arg_oAdvEvent);

        /// <summary>
        /// 新增AdvEvent
        /// </summary>
        /// <param name="arg_oAdvEvent">新增的AdvEvent</param>
        /// <returns>新增AdvEvent的ID; 0:新增失敗</returns>
        int AddAdvEvent(AdvEvent arg_oAdvEvent);
    }
}
