using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Redeem;
using TWNewEgg.EventRepoAdapters.Interface;

namespace TWNewEgg.Redeem.Service.CouponService
{
    public class EventFileRepository : IEventFile
    {
        private IEventRepoAdapter _EventRepoAdapter = null;

        public EventFileRepository(IEventRepoAdapter argEventRepo)
        {
            this._EventRepoAdapter = argEventRepo;
        }

        #region EventFile
        // <summary>
        /// 新增Event File
        /// </summary>
        /// <param name="EventFile">Event File物件</param>
        /// <returns>Event File Id</returns>
        public int addEventFile(Models.DomainModels.Redeem.EventFile arg_oEventFile)
        {
            int nEventFileId = 0;
            Models.DBModels.TWSQLDB.EventFile objDbEventFile = null;
            arg_oEventFile.createdate = DateTime.Now;

            try
            {
                objDbEventFile = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.EventFile>(arg_oEventFile);
                this._EventRepoAdapter.CreateEventFile(objDbEventFile);
                nEventFileId = objDbEventFile.id;
            }
            catch(Exception ex)
            {
                nEventFileId = 0;
            }
            return nEventFileId;
        }//end addEventFile

        /// <summary>
        /// 修改Event File
        /// </summary>
        /// <param name="EventFile">Event File物件</param>
        /// <returns>true: 修改成功, false: 修改失敗</returns>
        public bool editEventFile(Models.DomainModels.Redeem.EventFile arg_oEventFile)
        {
            Models.DBModels.TWSQLDB.EventFile oDbEventFile = null;
            bool bExec = false;

            try
            {
                oDbEventFile = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.EventFile>(arg_oEventFile);
                this._EventRepoAdapter.UpdateEventFile(oDbEventFile);
                bExec = true;
            }
            catch (Exception ex)
            {
                bExec = false;
            }

            return bExec;
        }//end editEventFile

        /// <summary>
        /// 僅修改EventFile的EventId, 其他欄位(含updatedate, updateuser)皆不修改
        /// </summary>
        /// <param name="arg_nEventFileId"></param>
        /// <param name="arg_nEventId"></param>
        /// <returns></returns>
        public bool editEventFileEventId(int arg_nEventFileId, int arg_nEventId)
        {
            if (arg_nEventId <= 0 || arg_nEventId == 0)
                return false;

            bool bExec = false;
            Models.DBModels.TWSQLDB.EventFile oDbEventFile = null;

            oDbEventFile = this._EventRepoAdapter.EventFile_GetAll().Where(x => x.id == arg_nEventFileId).FirstOrDefault();
            if (oDbEventFile != null)
            {
                oDbEventFile.eventid = arg_nEventId;
                try
                {
                    this._EventRepoAdapter.UpdateEventFile(oDbEventFile);
                    bExec = true;
                }
                catch (Exception ex)
                {
                    bExec = false;
                }

            }//end if

            return bExec;
        }//end editEventFileEvent()

        /// <summary>
        /// 取得EventFile
        /// </summary>
        /// <param name="EventFileId">Event File Id</param>
        /// <returns>Event File or null;</returns>
        public Models.DomainModels.Redeem.EventFile getEventFile(int arg_nEventFileId)
        {
            if(arg_nEventFileId <= 0)
                return null;

            Models.DBModels.TWSQLDB.EventFile objDbEventFile = null;
            Models.DomainModels.Redeem.EventFile oEventFile = null;

            objDbEventFile = this._EventRepoAdapter.EventFile_GetAll().Where(x=>x.id==arg_nEventFileId).FirstOrDefault();
            if(objDbEventFile != null)
            {
                oEventFile = ModelConverter.ConvertTo<Models.DomainModels.Redeem.EventFile>(objDbEventFile);
            }

            return oEventFile;
        }//end getEventFile()

        public List<Models.DomainModels.Redeem.EventFile> GetEventFiles(List<int> argListEventFileId)
        {
            if (argListEventFileId == null || argListEventFileId.Count <= 0)
            {
                return null;
            }

            List<Models.DomainModels.Redeem.EventFile> listEventFile = null;
            List<Models.DBModels.TWSQLDB.EventFile> listDbEventFile = null;

            listDbEventFile = this._EventRepoAdapter.EventFile_GetAll().Where(x => argListEventFileId.Contains(x.id)).ToList();

            if (listDbEventFile != null && listDbEventFile.Count > 0)
            {
                listEventFile = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.EventFile>>(listDbEventFile);
            }

            return listEventFile;
        }
        #endregion

        #region EventTempImport
        public void AddEventTempImport(Models.DomainModels.Redeem.EventTempImport argObjEventImport)
        {
            if (argObjEventImport == null)
            {
                return;
            }

            Models.DBModels.TWSQLDB.EventTempImport objDbImport = null;
            try
            {
                objDbImport = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.EventTempImport>(argObjEventImport);
                this._EventRepoAdapter.CreateEventTempImport(objDbImport);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            
        }

        public void AddRangeEventTempImport(List<Models.DomainModels.Redeem.EventTempImport> argListEventImport)
        {
            if (argListEventImport == null)
            {
                return;
            }

            List<Models.DBModels.TWSQLDB.EventTempImport> listDbImport = null;

            try
            {
                listDbImport = ModelConverter.ConvertTo <List<Models.DBModels.TWSQLDB.EventTempImport>>(argListEventImport);
                this._EventRepoAdapter.CreateRangeEventTempImport(listDbImport);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }

        public bool UpdateEventTempImport(Models.DomainModels.Redeem.EventTempImport argObjEventImport)
        {
            if (argObjEventImport == null)
            {
                return false;
            }

            bool boolExec = false;
            Models.DBModels.TWSQLDB.EventTempImport objDbImport = null;

            try
            {
                objDbImport = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.EventTempImport>(argObjEventImport);
                boolExec = this._EventRepoAdapter.UpdateEventTempImport(objDbImport);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }


            return boolExec;
        }

        public Models.DomainModels.Redeem.EventTempImport GetEventTempImportById(int argNumId, int argNumEventId)
        {
            Models.DomainModels.Redeem.EventTempImport objImport = null;
            Models.DBModels.TWSQLDB.EventTempImport objDbImport = null;

            objDbImport = this._EventRepoAdapter.EventTempImport_GetAll().Where(x => x.id == argNumId && x.event_id == argNumEventId).FirstOrDefault();

            if (objDbImport != null)
            {
                objImport = ModelConverter.ConvertTo<Models.DomainModels.Redeem.EventTempImport>(objDbImport);
            }

            return objImport;
        }
        #endregion
    }//end class
}//end namespace
