using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;

namespace TWNewEgg.AdvService.Service
{
    public interface IAdvEventDB
    {
        #region AdvEvent

        /// <summary>
        /// Dispose
        /// </summary>
        void Dispose();

        /// <summary>
        /// Get AdvEvent from db by AdvType and start date or end date
        /// </summary>
        /// <param name="advType">ENUM AdvType2 from AdvEvent.cs</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        /// <returns>List of AdvEvent</returns>
        List<AdvEvent> GetAdvEventByAdvType(int advType, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Get AdvEvent from db by AdvType and start date or end date
        /// </summary>
        /// <param name="advType">advType</param>
        /// <param name="closeDate">結束時間</param>
        /// <returns>AdvEvent object</returns>
        AdvEvent GetAdvEventFromCloseDate(int advType, DateTime closeDate);

        /// <summary>
        /// Get AdvEventType   by Code.
        /// </summary>
        /// <param name="advEventTypeCodes">list of advEventTypeCodes</param>
        /// <param name="takeNumber">int</param>
        /// <param name="datetimeNow">datetime</param>
        /// <returns>List of AdvEventType</returns>
        List<AdvEventType> GetAdvEventTypes(List<int> advEventTypeCodes, int takeNumber, DateTime? datetimeNow);

        /// <summary>
        /// Add a new advevent into DB, and return the advevent which saved in DB
        /// </summary>
        /// <param name="newAdvEvent">新增的AdvEvent</param>
        /// <returns>已新增並含有ID的AdvEvent</returns>
        AdvEvent AddNewAdvEvent(AdvEvent newAdvEvent);

        /// <summary>
        /// Update a advevent which already in DB.
        /// </summary>
        /// <param name="newAdvEvent">要修改AdvEvent</param>
        /// <returns>修改後的AdvEvent</returns>
        AdvEvent UpdateAdvEvent(AdvEvent newAdvEvent);

        /// <summary>
        /// Get advevents from DB by IDs.
        /// </summary>
        /// <param name="ids">ids</param>
        /// <returns>AdvEvent的列表</returns>
        List<AdvEvent> GetAdvEventByIDs(List<int> ids);

        /// <summary>
        /// Delete a advevent by ID.
        /// </summary>
        /// <param name="numId">id</param>
        /// <returns>true or false</returns>
        bool DeleteAdvEvent(int numId);

        /// <summary>
        /// Delete advevents by IDs
        /// </summary>
        /// <param name="ids">list of id</param>
        /// <returns>刪除結果</returns>
        string DeleteAdvEvents(List<int> ids);

        /// <summary>
        /// 根據AdvEventType取得旗下的所有AdvEvent
        /// </summary>
        /// <param name="arg_nAdvTypeId">AdvEventType.ID</param>
        /// <returns>null or List of AdvEvent</returns>
        List<AdvEvent> GetAllAdvEventByAdvEventTypeId(int arg_nAdvTypeId);

        /// <summary>
        /// 根據AdvEventType取得旗下的所有上線的AdvEvent
        /// </summary>
        /// <param name="arg_AdvEventTypeId">AdvEventId</param>
        /// <returns>null or List of AdvEvent</returns>
        List<AdvEvent> GetActiveAdvEventByAdvEventTypeId(int arg_AdvEventTypeId);

        /// <summary>
        /// 修改AdvEvent
        /// </summary>
        /// <param name="arg_oAdvEvent">要修改的AdvEvent</param>
        /// <param name="arg_numFlag">用來overload的flag</param>
        /// <returns>true:修改成功; false: 修改失敗</returns>
        bool UpdateAdvEvent(AdvEvent arg_oAdvEvent, int arg_numFlag);

        /// <summary>
        /// 新增AdvEvent
        /// </summary>
        /// <param name="arg_oAdvEvent">新增的AdvEvent</param>
        /// <returns>新增AdvEvent的ID; 0:新增失敗</returns>
        int AddAdvEvent(AdvEvent arg_oAdvEvent);
        #endregion

        #region AdvEventType

        /// <summary>
        /// 新增AdvEvent
        /// </summary>
        /// <param name="arg_oAdvEventType">新增的AdvEvent物件</param>
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
        /// <param name="arg_strCSS">CSS</param>
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
        /// 根據Country Code及AdvType取得AdvEventType的列表
        /// </summary>
        /// <param name="arg_numCountryId">Country ID</param>
        /// <param name="arg_numAdvType">AdvType</param>
        /// <returns>AdvEventType列表或是null</returns>
        List<AdvEventType> GetAdvEventTypeListByCountryAndAdvType(int arg_numCountryId, int arg_numAdvType);

        /// <summary>
        /// 根據Country Code及AdvType取得有效期間內的AdvEventType的列表
        /// </summary>
        /// <param name="arg_numCountryId">Country Id</param>
        /// <param name="arg_numAdvType">Convert enum AdvEventType.AdvTypeOption</param>
        /// <returns>List of AdvEventType</returns>
        List<AdvEventType> GetActiveAdvEventTypeListByCountryAndAdvType(int arg_numCountryId, int arg_numAdvType);

        /// <summary>
        /// 根據AdvType取得有效期間內的AdvEventType的列表
        /// </summary>
        /// <param name="arg_numAdvType">Convert enum AdvEventType.AdvTypeOption</param>
        /// <returns>List of AdvEventType</returns>
        List<AdvEventType> GetActiveAdvEventTypeListByAdvType(int arg_numAdvType);
        #endregion

    }
}
