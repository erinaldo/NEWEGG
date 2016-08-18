using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.EventRepoAdapters.Interface;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.EventRepoAdapters
{
    public class EventRepoAdapter:IEventRepoAdapter
    {
        private IRepository<Event> _eventdata;
        private IRepository<EventFile> _EventFileRepo;
        private IRepository<EventTempImport> _EventTempFileRepo;
        public EventRepoAdapter(IRepository<Event> eventdata, IRepository<EventFile> argEventFileRepo, IRepository<EventTempImport> argEventTempImport)
        {
            this._eventdata = eventdata;
            this._EventFileRepo = argEventFileRepo;
            this._EventTempFileRepo = argEventTempImport;
        }

        #region Event
        public IQueryable<Event> Event_GetAll()
        {
            return this._eventdata.GetAll();
        }

        public void CreateEvent(Event argObjEvent)
        {
            try
            {
                this._eventdata.Create(argObjEvent);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }

        public bool UpdateEvent(Event argObjEvent)
        {
            bool boolExec = false;

            Event objOldEvent = null;

            try
            {
                objOldEvent = this._eventdata.GetAll().Where(x => x.id == argObjEvent.id).FirstOrDefault();
                if (objOldEvent != null)
                {
                    ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.Event, Models.DBModels.TWSQLDB.Event>(argObjEvent, objOldEvent);
                    this._eventdata.Update(objOldEvent);
                    boolExec = true;
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return boolExec;
        }

        public IQueryable<Event> GetEventById(int argNumId)
        {
            IQueryable<Event> queryResult = null;

            queryResult = this.Event_GetAll().Where(x => x.id == argNumId);

            return queryResult;
        }

        #endregion

        #region eventfile
        public IQueryable<Models.DBModels.TWSQLDB.EventFile> EventFile_GetAll()
        {
            IQueryable<EventFile> queryResult = null;

            try
            {
                queryResult = this._EventFileRepo.GetAll();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return queryResult;
        }
        public void CreateEventFile(Models.DBModels.TWSQLDB.EventFile argObjEventFile)
        {
            try
            {
                this._EventFileRepo.Create(argObjEventFile);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }

        public bool UpdateEventFile(Models.DBModels.TWSQLDB.EventFile argObjEventFile)
        {
            bool boolExec = false;

            Models.DBModels.TWSQLDB.EventFile objDbEventFile = null;

            objDbEventFile = this._EventFileRepo.GetAll().Where(x => x.id == argObjEventFile.id).FirstOrDefault();
            if (objDbEventFile != null)
            {
                try
                {
                    ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.EventFile, Models.DBModels.TWSQLDB.EventFile>(argObjEventFile, objDbEventFile);
                    this._EventFileRepo.Update(objDbEventFile);
                    boolExec = true;
                }
                catch (Exception ex)
                {
                    throw new NotImplementedException(ex.Message, ex);
                }
            }

            return boolExec;
        }
        #endregion

        #region eventtempimport
        public IQueryable<Models.DBModels.TWSQLDB.EventTempImport> EventTempImport_GetAll()
        {
            IQueryable<EventTempImport> queryResult = null;

            try
            {
                queryResult = this._EventTempFileRepo.GetAll();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return queryResult;
        }

        public void CreateEventTempImport(Models.DBModels.TWSQLDB.EventTempImport argObjEventTempFile)
        {
            try
            {
                this._EventTempFileRepo.Create(argObjEventTempFile);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }

        public void CreateRangeEventTempImport(List<Models.DBModels.TWSQLDB.EventTempImport> argListEventTempFile)
        {
            try
            {
                this._EventTempFileRepo.CreateRange(argListEventTempFile);
            }
            catch(Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }

        public bool UpdateEventTempImport(Models.DBModels.TWSQLDB.EventTempImport argObjEventTempFile)
        {
            bool boolExec = false;
            Models.DBModels.TWSQLDB.EventTempImport objDbImport = null;

            objDbImport = this._EventTempFileRepo.GetAll().Where(x => x.event_id == argObjEventTempFile.event_id && x.account_id == argObjEventTempFile.account_id).FirstOrDefault();
            if (objDbImport != null)
            {
                try
                {
                    ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.EventTempImport, Models.DBModels.TWSQLDB.EventTempImport>(argObjEventTempFile, objDbImport);
                    this._EventTempFileRepo.Update(objDbImport);
                    boolExec = true;
                }
                catch (Exception ex)
                {
                    throw new NotImplementedException(ex.Message, ex);
                }
            }

            return boolExec;
        }
        #endregion

        
    }
}
