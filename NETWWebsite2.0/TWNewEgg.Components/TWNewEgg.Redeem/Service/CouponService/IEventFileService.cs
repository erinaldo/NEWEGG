using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels;

namespace TWNewEgg.Redeem.Service.CouponService
{
    public interface IEventFile
    {
        #region EventFile
        /// <summary>
        /// 新增Event File
        /// </summary>
        /// <param name="EventFile">Event File物件</param>
        /// <returns>Event File Id</returns>
        int addEventFile(Models.DomainModels.Redeem.EventFile arg_oEventFile);

        /// <summary>
        /// 修改Event File
        /// </summary>
        /// <param name="EventFile">Event File物件</param>
        /// <returns>true: 修改成功, false: 修改失敗</returns>
        bool editEventFile(Models.DomainModels.Redeem.EventFile arg_oEventFile);

        /// <summary>
        /// 僅修改EventFile的EventId, 其他欄位(含updatedate, updateuser)皆不修改
        /// </summary>
        /// <param name="arg_nEventFileId"></param>
        /// <param name="arg_nEventId"></param>
        /// <returns></returns>
        bool editEventFileEventId(int arg_nEventFileId, int arg_nEventId);

        /// <summary>
        /// 取得EventFile
        /// </summary>
        /// <param name="EventFileId">Event File Id</param>
        /// <returns>Event File or null;</returns>
        Models.DomainModels.Redeem.EventFile getEventFile(int arg_nEventFileId);

        /// <summary>
        /// 取得EventFile清單
        /// </summary>
        /// <param name="argListEventFileId"></param>
        /// <returns></returns>
        List<Models.DomainModels.Redeem.EventFile> GetEventFiles(List<int> argListEventFileId);
        #endregion

        #region EventTempImport

        /// <summary>
        /// Add EventTempImport
        /// </summary>
        /// <param name="argObjEventImport"></param>
        /// <returns></returns>
        void AddEventTempImport(Models.DomainModels.Redeem.EventTempImport argObjEventImport);

        /// <summary>
        /// Add EventTempImport
        /// </summary>
        /// <param name="argListEventImport"></param>
        void AddRangeEventTempImport(List<Models.DomainModels.Redeem.EventTempImport> argListEventImport);

        /// <summary>
        /// update EventTempImport
        /// </summary>
        /// <param name="argObjEventImport"></param>
        /// <returns></returns>
        bool UpdateEventTempImport(Models.DomainModels.Redeem.EventTempImport argObjEventImport);

        /// <summary>
        /// 根據Id取得EventTempImport
        /// </summary>
        /// <param name="argNumId">Id</param>
        /// <param name="argNumEventId">EventId</param>
        /// <returns></returns>
        Models.DomainModels.Redeem.EventTempImport GetEventTempImportById(int argNumId, int argNumEventId);
        #endregion
    }//end interface
}//end namespace
