using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.EventRepoAdapters.Interface
{
    public interface IEventRepoAdapter
    {
        #region event
        IQueryable<Event> Event_GetAll();

        /// <summary>
        /// 新增Event
        /// </summary>
        /// <param name="argObjEvent">要新增的Event</param>
        void CreateEvent(Event argObjEvent);

        /// <summary>
        /// 修改Event
        /// </summary>
        /// <param name="argObjEvent">要修改的Event</param>
        /// <returns>update success return true, else return false</returns>
        bool UpdateEvent(Event argObjEvent);

        /// <summary>
        /// 根據EventId取得Event
        /// </summary>
        /// <param name="argNumId">Event Id</param>
        /// <returns></returns>
        IQueryable<Event> GetEventById(int argNumId);
        #endregion

        #region eventfile
        IQueryable<Models.DBModels.TWSQLDB.EventFile> EventFile_GetAll();
        void CreateEventFile(Models.DBModels.TWSQLDB.EventFile argObjEventFile);
        bool UpdateEventFile(Models.DBModels.TWSQLDB.EventFile argObjEventFile);
        #endregion

        #region eventtempimport
        IQueryable<Models.DBModels.TWSQLDB.EventTempImport> EventTempImport_GetAll();
        void CreateEventTempImport(Models.DBModels.TWSQLDB.EventTempImport argObjEventTempFile);
        void CreateRangeEventTempImport(List<Models.DBModels.TWSQLDB.EventTempImport> argListEventTempFile);
        bool UpdateEventTempImport(Models.DBModels.TWSQLDB.EventTempImport argObjEventTempFile);

        #endregion

    }
}
